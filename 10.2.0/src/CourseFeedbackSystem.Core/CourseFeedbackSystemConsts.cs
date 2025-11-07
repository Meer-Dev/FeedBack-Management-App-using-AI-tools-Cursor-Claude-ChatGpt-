using CourseFeedbackSystem.Debugging;

namespace CourseFeedbackSystem;

public class CourseFeedbackSystemConsts
{
    public const string LocalizationSourceName = "CourseFeedbackSystem";

    public const string ConnectionStringName = "Default";

    public const bool MultiTenancyEnabled = true;


    /// <summary>
    /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
    /// </summary>
    public static readonly string DefaultPassPhrase =
        DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "eab880e76ddd4f638ce2ecb35f6f25c1";
}
