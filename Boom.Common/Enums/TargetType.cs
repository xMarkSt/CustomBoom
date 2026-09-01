namespace Boom.Common.Enums;

// Mirrors the stable, seeded ids in the `targets` table (see Scripts/targets.sql).
// Target.Type is display text and can be renamed/localized independently of these ids.
public enum TargetType
{
    FastestTime = 1,
    AllPickups = 2,
    CompleteRightButton = 3,
    NoCoins = 4,
    AllBombs = 5,
    Pickups = 6,
    TouchBall = 7,
    Bombs = 8,
    Rockets = 9,
    UseRocket = 10,
    OnlyOneRocket = 11,
    FinishWithRocket = 12,
    Springboards = 13,
    AllJumpboards = 14,
    TouchStar = 15,
    BreakBall = 16,
    TouchBowlingPin = 17
}
