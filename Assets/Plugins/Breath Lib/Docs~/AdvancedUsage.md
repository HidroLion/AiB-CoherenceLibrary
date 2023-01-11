# Advanced Usage
This page delves into the usage of the breath library and how create game interactions.

# Overview
At a high level, the breath library is made up of three main components:

### **The Hardware Input:**

There are several built in hardware inputs defined by a `IBreathDetector`. Multiple types of hardware can be used at once to improve the accuracy, and can change depending on the platform, user preference, or hardware accessability.

The `DetectorController` is responsible for collecting and managing the input from the hardware. All of the hardware input is collected into a `BreathStream`. For ease of use, the `DetectorController` can and should be managed by a `DetectorManager` component, which will automatically initialize and update the controller.

### **The Breath Data and Patterns:**

<!-- A `BreathSample` 

A `Pattern` is a simplified animation, representing a breathing technique. `Patterns` consist of `Keyframes` which state  -->

WIP