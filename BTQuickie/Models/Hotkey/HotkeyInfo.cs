namespace BTQuickie.Models.Hotkey;


public record HotkeyInfo(string Key, string Modifiers, int Id, string Description) : Hotkey(Key, Modifiers)
{
    public static HotkeyInfo Empty => new(string.Empty, string.Empty, -1, string.Empty);
}