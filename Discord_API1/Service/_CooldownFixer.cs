namespace Discord_API1.Service
{
    /// <summary>
    /// This class will be later injected in our service.
    /// Created in order to close issue  ---- https://github.com/mad-redhead/SpotifyBot_testbot/issues/5 --- issue context
    /// 
    /// </summary>
    public class _CooldownFixer
    {
        public int use_times;
        public _CooldownFixer()
        {
            use_times = 5;
        }

        public void aaa()
        {
            var a = 3;
        }
    }
}