namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class Donate : AbstractCommand {

        public override string trigger { get { return "donate"; } }
        public override string description { get { return null; } }
        public override string example { get { return null; } }

        public override void Run(string[] args) {
            System.Diagnostics.Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=BTMLSDQULZ852");
        }
    }
}
