namespace NoatunCrewing.Enums;

// Determines which backing store a crew record lives in.
// Filipino records: external AMS DB, read-only.
// Kenyan records: NoatunMGT DB, full read/write.
public enum CrewSource
{
    Ams = 0,
    NoatunCrewing = 1
}
