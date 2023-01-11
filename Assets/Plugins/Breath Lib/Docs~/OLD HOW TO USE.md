# Breath Library: How to Use
The breath library is a plugin that enables easy to configure Breath Detection in any project. The core features include:
- Built-in and extendable [compatibility with different hardware](#hardware-and-devices-input).
- Configurable [sample collection](#correction-blending-aggregation-and-sessions). Examples: Sample rates and desired accuracy.
- Automatically generated [session data](#session-data-and-database) for long-term user data.
- Built-in and creatable [breath patterns](#breath-patterns) for precise game interactions.
- Built-in and extendable [pattern correlations](#pattern-correlation) for target vs detected breaths, including:
	- Progression based: Breath patterns only progress when the user matches the breath
	- Accuracy based: Breath patterns move at a set pace and score the accuracy of the detected data.
	- *More to come!*
- Built-in [wrappers, helpers, and samples](#wrappers-helpers-and-samples).

This document is intended for the Unity Plugin, but many concepts are the same for the other platforms. For a fully playable experience for sampling the plugin, please visit the [AiB-Ladder Project](https://github.com/Versebuilding/AiB-Ladder).

<!-- TODO For additional help getting started go to TODO -->

## Important Naming Conventions
All of the below try to fit within standard usage, but ultimately follow a non-official schema:
- **Sample:** General refers to a group of data that is collected at a specific point in time. Samples should always be collected independent from one another (i.e. should not rely on past samples).
- **Manager:** A script/object that oversees and performs a given task. Usually, managers will *be told* what to do and when. Managers are not aware of time nor how they are being used.
- **Controller:** A script/object that exists as a part of the game. Usually, controllers are in charge of configuring and starting/stopping/updating managers.

## Hardware and Devices (Input)

## Breath Detection and Input Processing

## Breath Samples
**Breath Samples** are a form of breath sample. Each individual sample will correlate to an exact point in time. These samples should be collected independent of other samples collected around it. See [Breath Detection and Input Processing](#breath-detection-and-input-processing) for more information on how these samples are created.

The Breath samples class is an <u>extended</u> <u>fixed sized array</u> of <u>nullable float values</u>.
- **Fixed Size Array**: A list of values that can be iterated and indexed. This list is predefined and cannot have values added or removed. This is useful for iterating through all sample data w/out needing to pull each individual value:
```C#
var sample = new BreathSample();
foreach(float? data in sample) {
	// Do something, like summing differences
}
/* Versus non array */
float? data0 = sample.In;
float? data1 = sample.No;
float? data2 = sample.Mouth;
float? data3 = sample.Pitch;
float? data4 = sample.Volume;
```
- **Nullable Float**: A float, decimal number, that can also be `null`. We use a null value to indicate that a particular sample value, like pitch, is not being used. "Not being used" can mean that a detection mechanic cannot measure, or that a breath pattern does not care about this value. See the [Microsoft dotnet docs](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/nullable-value-types) for more info.
```C#
var sample = new BreathSample();
sample.In = 0.8f; // Being used.
sample.No = 0.0f; // Being used.
sample.Pitch = null; // (explicitly) Not being used.
// sample.Volume is also (implicitly) not being used because it is never set.
```
- **Extended**: Has useful additions that make coding easier. Mainly, the class provides properties for data so programers do not need to remember what each index of the array is used for:
```C#
var sample = new BreathSample();
// The following all do the same:
sample[0] = 0.6f;
sample.No = 0.6f;
sample.Yes = 0.4f;
```

## Correction, Aggregation, and Sessions
Once a hardware device has input and that input is formatted into a sample, it will be requested and passed off to the **CAS Manager** (Correction, Aggregation, and Sessions).

## Aggregation Data

## Session Data and Database

## Breath Patterns

## Pattern Correlation

## Wrappers, Helpers, and Samples

## Game Side Usage and Presentation

