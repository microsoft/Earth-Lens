# Earth Lens Models

This folder contains the Object Detection model we used in EarthLens, for demostration purpose only. This folder contains two models:

- An object detection model trained using [TensorFlow Object Detection API](https://github.com/tensorflow/models/tree/master/research/object_detection). In order for the model to run on an edge device such as iPad, we chose to use a light weight feature extractor, [MobileNet](https://arxiv.org/abs/1704.04861), as well as a one-stage object detector, [Single Shot MultiBox Detector](https://arxiv.org/abs/1512.02325), to accomplish the goal.

- Since we specifically target iOS system, we also need to convert the above model into the iOS specific format, i.e. CoreML. 

- You also need to "compile" the `.mlmodel` into `.mlmodelc` format, in order to be finally used in the iOS project. As mentioned in the main README file, please refer to the [Xamarin document](https://docs.microsoft.com/en-us/xamarin/ios/platform/introduction-to-ios11/coreml) for details.

# DISCLAIMERS

The model is intended for demonstration purposes only.  

THE SOFTWARE CODE IS PROVIDED “AS IS” WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL MICROSOFT OR ITS LICENSORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SOFTWARE CODE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 