# Earth Lens

[Earth Lens](https://github.com/Microsoft/Earth-Lens) leverages Microsoft’s AI tools to detect objects in aerial imagery and translate raw image data into digestible insights for researchers. With this tool, researchers will be able to automatically extract information from a large dataset and have the analysis presented in a useful and interactive view. As a result, this tool can be useful in various industrial or agricultural applications to further Microsoft’s AI For Earth commitment to sustainability.

The application utilizes Apple's [CoreML framework](https://developer.apple.com/documentation/coreml) to run the machine learning model locally on the iPad, showcasing the power and capability of the Intelligent Edge. Users are able to upload images, process the locally without the internet, and then scroll through and analyze the data. Earth Lens helps users automatically aggregate, count, classify and label the objects in satellite images. Colored bounding boxes are drawn around detected objects, which users can toggle on and off to control the information displayed. A time series analysis view allows users to look at images over time alongside a data visualization chart to identify trends and patterns.

# Device Compatibility

This application runs on iPad devices with iOS 9.0 or above.

# Features

* Categorization / Filtering Sidebar
* Timeline Series Analysis with Data Visualizer
* Color Customization
* Observation Relabeling
* PDF Report Export
* Customized Settings

# Project Structure

## Xamarin

This folder contains the application Visual Studio solution, which consists of five separate projects:

### EarthLens

* Code that is platform-independent and common to multiple platforms
    * Class definitions that provide API for the application to represent CoreML model outputs
        * Observations detected by the CoreML model
        * Categories and mega-categories that classify detected observations
        * Chip and image entry for intermediate representations
        * Internal gallery section
    * Database
        * Database access objects (DAO) of images and observations
    * CoreML model
        * Database service to manage the database (e.g. remove / insert entries)
        * Environment service to manage environment variables for App Center
        * Graphic service for PDF report generation and export
        * Image analysis service that utilizes parallel computing to analyze images
        * Image encoding service to convert images to their base64 representations
        * JSON service to serialize and deserialize JSON format
        * Machine learning service for mathmetical functions related to ML
        * Post processing service to apply filtering and non-maximum suppression algorithm to results detected by the CoreML model

### EarthLens.iOS

* Core iOS application code and UI
    * Storyboards files, XIB files, etc. with view controllers
    * Icon assets
    * CoreML-based implementation of for the machine learning model integration
    * PDF generation service for PDF report export
    * Data visualization service to generate bar charts data visualizer

### EarthLens.Tests

* Unit testing project for the `EarthLens` project

### EarthLens.Tests.iOS

* Unit testing project for the `EarthLens.iOS` project

### EarthLens.TestUtils

* Common unit testing utility code that is common to both `EarthLens.Tests` and `EarthLens.Tests.iOS`

# Getting Started with Development

## Setup on Windows

### Windows 7 or above

1. Update your Windows if you are on a version older than Windows 7, as specified in the Xamarin [System requirements](https://docs.microsoft.com/en-us/xamarin/cross-platform/get-started/requirements).

### Visual Studio 2017 (Community, Professional, or Enterprise) and Xamarin 4.5.0 or above

1. Follow the [Windows installation instructions](https://docs.microsoft.com/en-us/xamarin/cross-platform/get-started/installation/windows#Installation) to install Visual Studio 2017 (Community, Professional, or Enterprise) and the Xamarin component on Windows.

### Mac Agent

1. Follow the official Xamarin documentation to [Pair to Mac for Xamarin.iOS Development](https://docs.microsoft.com/en-us/xamarin/ios/get-started/installation/windows/connecting-to-mac/).

## Setup on macOS

### macOS High Sierra 10.12 or above

1. Update your OS version if you are on a version older than 10.12, as specified in the Xamarin [System requirements](https://docs.microsoft.com/en-us/xamarin/cross-platform/get-started/requirements).

### Xcode 9 or above

1. Download and install Xcode from App Store on macOS.
2. Launch Xcode.
3. Read and accept the License Agreement.
4. Let Xcode install additional components (this includes command-line tools that Visual Studio needs).

### Visual Studio for Mac

1. Follow the official Microsoft documentation to [Setup and Install Visual Studio for Mac](https://docs.microsoft.com/en-us/visualstudio/mac/installation)
    * Make sure at least the following options are selected when choosing what to install
        * iOS
        * .NET Core

## Getting the Source Code

1. Clone the repository to your development machine:

```Bash
git clone https://github.com/Microsoft/Earth-Lens.git
```

2. Open `Xamarin/EarthLens.sln` in Visual Studio (for Windows) or Visual Studio for Mac (for macOS).

## Building/Running the App

### Running on a simulator

* Note for Windows users:
    * You can use a [Remoted iOS Simulator](https://docs.microsoft.com/en-us/xamarin/tools/ios-simulator) that is remotely connected to the Mac Agent and will launch on the Windows machine.
    * If you choose not to use a remoted iOS simulator, the simulator will launch on the Mac Agent.

1. In Visual Studio, select the following configuration:

    |               |                   |
    | ------------- | ----------------- |
    | Project       | `EarthLens.iOS`   |
    | Configuration | `Debug`           |
    | Platform      | `iPhoneSimulator` |
    | Device        | `<select iPad>`   |

2. Run the app on the simulator.

### Running on a physical device

1. Connect the physical iPad on which the app is to run to a Mac or Mac Agent.
2. If prompted to "Trust This Computer" on the iPad, select "Trust" and enter the iPad passcode.
3. If iTunes launches, wait for it to identify the device, unplug the device, and then plug the device back in.
4. For the first time, follow the Xamarin documentation to setup [Device Provisioning for Xamarin.iOS](https://docs.microsoft.com/en-us/xamarin/ios/get-started/installation/device-provisioning/).
    * For the product name, enter: `EarthLens`
    * For the bundle ID, enter: `com.companyname.EarthLens`
5. In Visual Studio, select the following configuration:

    |               |                    |
    | ------------- | ------------------ |
    | Project       | `EarthLens.iOS`    |
    | Configuration | `Debug`            |
    | Platform      | `iPhone`           |
    | Device        | `<your iPad name>` |

6. Run the app on the iPad.

## App Center (Optional)

Microsoft's [Visual Studio App Center](https://appcenter.ms/) is used for the development of Earth Lens. App Center is a collection of services that enable developers to better manage and ship applications on any platform. The following App Center services are used during the development of Earth Lens:

* [Build](https://docs.microsoft.com/en-us/appcenter/build/)
* [Analytics](https://docs.microsoft.com/en-us/appcenter/analytics/)
* [Diagnostics](https://docs.microsoft.com/en-us/appcenter/diagnostics/)

App Center is free to try out. To get started, visit [https://appcenter.ms/](https://appcenter.ms/) to get an App Secret key, and follow the [Get Started with Xamarin](https://docs.microsoft.com/en-us/appcenter/sdk/getting-started/xamarin) instructions for Xamarin.iOS.

Earth Lens uses [Build scripts](https://docs.microsoft.com/en-us/appcenter/build/custom/scripts/) and [Environment variables](https://docs.microsoft.com/en-us/appcenter/build/custom/variables/) to load the App Center App Secret key into Earth Lens during runtime. Under the `EarthLens.iOS` project, there is a `appcenter-pre-build.sh` file that is run each time a build is triggered on App Center, and an `Environment.txt` file where the App Secret key is entered.

App Center supports automated builds and distributions. For more information, check out [Building Xamarin apps for iOS](https://docs.microsoft.com/en-us/appcenter/build/xamarin/ios/).

## CoreML Integration

1. Acquire the compiled `.mlmodelc` format model of your original `.mlmodel` format model, as indicated [here](https://docs.microsoft.com/en-us/xamarin/ios/platform/introduction-to-ios11/coreml). Put the folder containing the compiled model under the `Resources` folder of the `EarthLens.iOS` project.
2. Acquire the model's anchors in the training process. Store them in a CSV file, and put the file under the `Resources` folder of the `EarthLens.iOS` project.
3. Make sure that the `ResourceName` (for the model path) , and `AnchorsResrouceName` (for the anchor file path) values in the `Constants.cs` file under the `EarthLens.iOS` project are correct.
4. Inspect the original `.mlmodel` file, and acquire the input and output feature names. Make sure that the `DefaultInputFeatureName` ,`ModelOutputBoxFeatureName` and `ModelOutputClassFeatureName` values in the `Constants.cs` file under the `EarthLens.iOS` project correspond to the feature names of your model.

# Third-Party Library Dependencies

* [LiteDB](https://github.com/mbdavid/LiteDB)
* [GMImagePicker](https://github.com/guillermomuntaner/GMImagePicker)
* [Newtonsoft](https://www.newtonsoft.com/json)
* [SkiaSharp](https://github.com/mono/SkiaSharp)
* [App Center SDK](https://docs.microsoft.com/en-us/appcenter/sdk/)
* [NUnit.Xamarin](https://github.com/nunit/nunit.xamarin)
* [Touch.Unit](https://github.com/spouliot/Touch.Unit)

# Contributing

This project welcomes contributions and suggestions. There are many ways to contribute to Earth Lens:

* [Submit bugs](https://github.com/Microsoft/Earth-Lens/issues) and help us verify fixes as they are checked in.
* Review the [source code changes](https://github.com/Microsoft/Earth-Lens/pulls).
* [Contribute bug fixes](https://github.com/Microsoft/Earth-Lens/pulls).

Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

# License

Earth Lens is released under the MIT License.

See [LICENSE](https://github.com/Microsoft/Earth-Lens/blob/master/LICENSE) for details.

# Acknowledgement

## Developer Interns

* [Seara Chen](https://github.com/SearaChen) ([LinkedIn](https://www.linkedin.com/in/seara-chen-985b29101/))
* [Alex Jordache](https://github.com/therealAJ) ([LinkedIn](https://www.linkedin.com/in/ajordache/))
* [Elsie Ju](https://github.com/Elsie4ever) ([LinkedIn](https://www.linkedin.com/in/jiachen-ju-17088b133/))
* [Yuchong Pan](https://github.com/yuchong-pan) ([LinkedIn](https://linkedin.com/in/yuchong-pan/))
* [Nelani Skantharajah](https://github.com/nelanis) ([LinkedIn](https://www.linkedin.com/in/nelani-skantharajah/))

## Designer Intern

* [Kelin Kaardal](https://www.kelinkaardal.com/) ([LinkedIn](https://www.linkedin.com/in/kelinkaardal/))

## Program Manager Intern

* [Michelle Chen](https://github.com/mchenco) ([LinkedIn](https://linkedin.com/in/mchenco))

## Coaches

* Andrii Kalinichenko ([LinkedIn](https://www.linkedin.com/in/kalinichenkoandrii/))
* Hailey Musselman ([LinkedIn](https://www.linkedin.com/in/hailey-musselman-8a0506ab/))
* Tejbir Sodhan ([LinkedIn](https://www.linkedin.com/in/tejbirsodhan/))

## Program Manager

* Stephane Morichere-Matte ([LinkedIn](https://www.linkedin.com/in/stephanemoricherematte/))

## Technical Advisor

* Mark Schramm ([LinkedIn](https://www.linkedin.com/in/markschramm/))
