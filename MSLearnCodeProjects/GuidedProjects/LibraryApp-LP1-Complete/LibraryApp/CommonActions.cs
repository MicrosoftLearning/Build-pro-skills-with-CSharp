
namespace LibraryApp.ConsoleApp;

[Flags]
public enum CommonActions
{
    Repeat = 0,
    Select = 1,
    Quit = 2,
    SearchPatrons = 4,
    ReturnLoanedBook = 8
}