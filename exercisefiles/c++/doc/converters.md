# C++ Converters Documentation

This document describes the different converters implemented in the project.

## Temperature Converter
- **Location:** `src/converters/temperature.h` / `temperature.cpp`
- **Purpose:** Converts temperature values between Celsius, Fahrenheit, and Kelvin.
- **Key Functions:**
  - `startFlow()`: Handles user interaction for temperature conversion.
  - `convertTemperature(double value, TemperatureUnit from, TemperatureUnit to)`: Converts a temperature value from one unit to another.

## Distance Converter
- **Location:** `src/converters/distance.h` / `distance.cpp`
- **Purpose:** Converts distance values between meters, feet, and yards.
- **Key Functions:**
  - `startFlow()`: Handles user interaction for distance conversion.
  - `convertDistance(double value, DistanceUnit from, DistanceUnit to)`: Converts a distance value from one unit to another.

## Usage
- The main program (`src/main.cpp`) allows the user to select the type of conversion (temperature or distance) and then interactively performs the conversion using the appropriate converter.

## Example
```
Select type of conversion:
[1] Temperature
[2] Distance
Enter choice: 2
... (distance conversion flow starts)
```

---

For more details, see the source files in `src/converters/`.
