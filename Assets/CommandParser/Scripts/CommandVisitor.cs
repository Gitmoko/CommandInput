using Command.Actions;
public interface CommandVisitor
{
    void Visit(Sequence seq);
    void Visit(SameTime same);
    void Visit(Hold hold);
    void Visit(Press press);

    void Visit(Release release);

    void Visit(Press_Dir press_dir);
    void Visit(Hold_Dir hold_dir);
    void Visit(Release_Dir release_dir);

}
