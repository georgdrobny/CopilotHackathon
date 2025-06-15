#include "distance.h"
#include <gtest/gtest.h>
#include <cmath>

using namespace DistanceConversion;

TEST(DistanceConverterTest, MetersToFeet) {
    EXPECT_NEAR(convertDistance(1.0, DistanceUnit::Meters, DistanceUnit::Feet), 3.28084, 1e-5);
}

TEST(DistanceConverterTest, MetersToYards) {
    EXPECT_NEAR(convertDistance(1.0, DistanceUnit::Meters, DistanceUnit::Yards), 1.09361, 1e-5);
}

TEST(DistanceConverterTest, FeetToMeters) {
    EXPECT_NEAR(convertDistance(3.28084, DistanceUnit::Feet, DistanceUnit::Meters), 1.0, 1e-5);
}

TEST(DistanceConverterTest, FeetToYards) {
    EXPECT_NEAR(convertDistance(6.0, DistanceUnit::Feet, DistanceUnit::Yards), 2.0, 1e-5);
}

TEST(DistanceConverterTest, YardsToMeters) {
    EXPECT_NEAR(convertDistance(1.09361, DistanceUnit::Yards, DistanceUnit::Meters), 1.0, 1e-5);
}

TEST(DistanceConverterTest, YardsToFeet) {
    EXPECT_NEAR(convertDistance(2.0, DistanceUnit::Yards, DistanceUnit::Feet), 6.0, 1e-5);
}

TEST(DistanceConverterTest, SameUnit) {
    EXPECT_NEAR(convertDistance(5.0, DistanceUnit::Meters, DistanceUnit::Meters), 5.0, 1e-5);
    EXPECT_NEAR(convertDistance(7.0, DistanceUnit::Feet, DistanceUnit::Feet), 7.0, 1e-5);
    EXPECT_NEAR(convertDistance(9.0, DistanceUnit::Yards, DistanceUnit::Yards), 9.0, 1e-5);
}

TEST(DistanceConverterTest, ZeroValue) {
    EXPECT_NEAR(convertDistance(0.0, DistanceUnit::Meters, DistanceUnit::Feet), 0.0, 1e-5);
}
