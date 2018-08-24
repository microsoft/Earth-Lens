using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CoreML;
using EarthLens.Models;
using EarthLens.Services;
using Foundation;
using SkiaSharp;

namespace EarthLens.iOS.Services
{
    public class CoreMLModel : IDisposable
    {
        private const string ModelResourceExt = Constants.ModelResourceExt;
        private const string ModelName = Constants.ResourceName;
        private const int NumClasses = Constants.NumClasses;
        private const int NumAnchorBox = Constants.NumAnchorBoxes;

        private MLModel _mlModel;
        private readonly string _inputFeatureName;

        /// <summary>
        /// A default constructor with default constructor value.
        /// </summary>
        public CoreMLModel()
        {
            var assetPath = NSBundle.MainBundle.GetUrlForResource(ModelName, ModelResourceExt);
            if (assetPath == null)
            {
                throw new FileNotFoundException();
            }
            _mlModel = MLModel.Create(assetPath, out _);
            _inputFeatureName = Constants.DefaultInputFeatureName;
        }

        /// <summary>
        /// Runs CoreML on the image in SKBitmap format .
        /// </summary>
        /// <returns>A <see cref="CategoryManager"/> that contains all observations detected from the specified
        /// <see cref="MLMultiArray"/>.</returns>
        /// <param name="input">The array representation of image after pre-processing </param>
        public CategoryManager Predict(MLMultiArray input)
        {
            MLMultiArray output1;
            MLMultiArray output2;
            (output1, output2) = RawPredict(input);

            // checking for expected dimensions
            if (output1.Shape == new nint[] { 4, NumAnchorBox, 1 })
            {
                // one anchoring box is represented by 4 points, no duplicating anchoring box
                return null;
            }
            if (output2.Shape == new nint[] { 1, 1, NumClasses, 1, NumAnchorBox })
            {
                return null;
            }

            // conversion from MLMultiArray to double readable content
            var outputFloats1 = new double[output1.Count]; // box prediction encoding 4 x 1917 x 1 
            for (var i = 0; i < output1.Count; i++)
            {
                outputFloats1[i] = (double)output1[i];
            }

            var outputFloats2 = new double[output2.Count]; // confidence score output matrix : 1 x 1 x 23 x 1 x 1917
            for (var i = 0; i < output2.Count; i++)
            {
                outputFloats2[i] = (double)output2[i];
            }

            output1.Dispose();
            output2.Dispose();

            var cm = new CategoryManager();

            for (var ibox = 0; ibox < NumAnchorBox; ibox++)
            {
                var scoresOfClasses = new List<double>();
                for (var iclass = 0; iclass < NumClasses; iclass++)
                {
                    var offsetIndex = iclass * NumAnchorBox + ibox;
                    var logit = outputFloats2[offsetIndex];
                    scoresOfClasses.Add(logit);                       
                }

                scoresOfClasses = MachineLearningService.Softmax(scoresOfClasses).ToList();

                for (var iclass = 0; iclass < NumClasses; iclass++)
                {
                    var confidence = scoresOfClasses[iclass];

                    var box = GenerateSKRect(
                        outputFloats1[BoxIndexOffset(0, ibox)],
                        outputFloats1[BoxIndexOffset(1, ibox)],
                        outputFloats1[BoxIndexOffset(2, ibox)],
                        outputFloats1[BoxIndexOffset(3, ibox)],
                        ibox);

                    if (iclass != 0 && IsWithinFrame(box))
                    {
                        var _ = new Observation(cm.GetOrCreate(iclass, ModelClassLabelEncoding.LookUpLabel(iclass)),
                            confidence, box);
                    }
                }
            }
            return cm;
        }

        /// <summary>
        /// Extract the direct output from MLCore without post-processing 
        /// </summary>
        /// <returns>A tuple contain the two matrices output by the model. First matrix: the box prediction encoding; second matrix: the class logit for each box.</returns>
        /// <param name="input"> The formatted matrix representation of image after proper transpose and normalization</param>
        private (MLMultiArray, MLMultiArray) RawPredict(MLMultiArray input)
        {
            var inputFeatureProvider = new ImageInputFeatureProvider(_inputFeatureName)
            {
                ImagePixelData = input
            };
            var resultBundle = _mlModel.GetPrediction(inputFeatureProvider, out _);
            var output1 = resultBundle.GetFeatureValue(Constants.ModelOutputBoxFeatureName).MultiArrayValue;
            var output2 = resultBundle.GetFeatureValue(Constants.ModelOutputClassFeatureName).MultiArrayValue;

            inputFeatureProvider.Dispose();

            return (output1, output2);
        }

        /// <summary>
        /// A wrapper class that formats the input image matrix data for CoreML consumption.
        /// </summary>
        private class ImageInputFeatureProvider : NSObject, IMLFeatureProvider
        {
            private readonly string _inputFeatureName;

            internal ImageInputFeatureProvider(string inputFeatureName)
            {
                _inputFeatureName = inputFeatureName;
            }

            public MLMultiArray ImagePixelData { private get; set; }
            public NSSet<NSString> FeatureNames => new NSSet<NSString>(new NSString(_inputFeatureName));
            public MLFeatureValue GetFeatureValue(string featureName) => _inputFeatureName.Equals(featureName)
                    ? MLFeatureValue.Create(ImagePixelData)
                    : MLFeatureValue.Create(0);
        }

        /// <summary>
        /// Converts the box prediction encoding from the model to SKRect bounding boxes for display.
        /// </summary>
        /// <returns>SKRect Rectangle ready for display on image.</returns>
        /// <param name="ty">The y-coordinate of the box prediction encoding output by the model.</param>
        /// <param name="tx">The x-coordinate of the box prediction encoding output by the model.</param>
        /// <param name="th">The height of the box prediction encoding output by the model.</param>
        /// <param name="tw">The width of the box prediction encoding output by the model.</param>
        /// <param name="ibox">The index of the box encoding referring out of total number of anchoring boxses.</param>
        private static SKRect GenerateSKRect(double ty, double tx, double th, double tw, int ibox)
        {
            var anchor  = Anchors.GetBoxCoordinate(ibox);

            var yACtr = (anchor.YMin + anchor.YMax) / 2.0;
            var xACtr = (anchor.XMin + anchor.XMax) / 2.0;

            var ha = anchor.YMax - anchor.YMin;
            var wa = anchor.XMax - anchor.XMin;
            ty = ty / Constants.ModelYScale;
            tx = tx / Constants.ModelXScale;
            th = th / Constants.ModelHeightScale;
            tw = tw / Constants.ModelWidthScale;

            var w = Math.Exp(tw) * wa;
            var h = Math.Exp(th) * ha;

            var yCtr = ty * ha + yACtr;
            var xCtr = tx * wa + xACtr;

            var yMin = yCtr - h / 2.0;       
            var xMin = xCtr - w / 2.0;
            var yMax = yCtr + h / 2.0;
            var xMax = xCtr + w / 2.0;

            // transpose the result matrix
            return new SKRect((float)(1 - xMax), (float)(1 - yMax), (float)(1 - xMin), (float)(1 - yMin));
        }

        /// <summary>
        /// Checks if the bounding boxes is within the ratio frame of the image.
        /// </summary>
        /// <returns>The label.</returns>
        private static bool IsWithinFrame(SKRect input)
        {
            var xMax = input.Right;
            var xMin = input.Left;
            var yMax = input.Bottom;
            var yMin = input.Top;

            return !(1 < xMax) && !(xMin < 0) && !(1 < yMax) && !(yMin < 0);
        }

        /// <summary>
        /// Calculates the index offset for the box prediciton matrix (output1).
        /// </summary>
        /// <returns>The index offset in the <see cref="MLMultiArray"/>.</returns>
        private static int BoxIndexOffset(int i, int iBox)
        {
            return i * NumAnchorBox + iBox;
        }

        /// <summary>
        /// Disposes the current <see cref="CoreMLModel"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CoreMLModel()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (_mlModel == null) return;
            _mlModel.Dispose();
            _mlModel = null;
        }
    }
}
