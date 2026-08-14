# Countdown Clock

Countdown Clock is a small Visual Basic .NET Windows Forms program that displays the years, months, days, hours, minutes, and seconds remaining until a user-specified date and time.

![Countdown Clock screenshot](Countdown_Clock_screencap.png)

Years ago I used a small program called **RCT** (Retirement Countdown Timer) that displayed a countdown until retirement. RCT depended on the old Visual Basic 5.0 run-time libraries, so I created this Visual Basic .NET replacement. Its appearance is deliberately similar to RCT, but Countdown Clock is general-purpose: the target date, time, and window-title message are configurable.

## Configuration

Countdown Clock reads its target from [`EndDate.txt`](EndDate.txt). The first line must have this form:

```text
YYYY-MM-DD HH:MM:SS,Message
```

For example:

```text
2099-01-01 00:00:00,Retirement Begins In ...
```

The date and time are interpreted as **local wall-clock time in the computer's current local time zone**. No time-zone field is required in `EndDate.txt`. The text after the comma is displayed in the program's title bar.

When the file is read, Countdown Clock validates the local date and time and converts it to UTC using the local time-zone rules. A target that falls in a daylight-saving transition gap (an invalid local time) or in a repeated hour (an ambiguous local time) is rejected rather than silently mapped to the wrong instant.

`EndDate.txt` must be in the program's current working directory when the executable is started. When the target instant has been reached, the countdown timer stops.

## Prebuilt executable

The prebuilt Windows executable is located in [`release/`](release/):

```text
release/Countdown Clock.exe
release/Countdown Clock.exe.config
release/EndDate.txt
```

## Building from source

The project targets **.NET Framework 4.8** and uses Windows Forms.

1. Open [`CountdownClock.sln`](CountdownClock.sln) in Visual Studio.
2. Select the desired build configuration.
3. Build the solution.
4. Copy or edit `EndDate.txt` in the directory from which the executable will be run.

Normal Visual Studio-generated directories and per-user files such as `.vs/`, `bin/`, `obj/`, `*.suo`, and `*.user` are intentionally excluded from version control by [`.gitignore`](.gitignore).

## Date calculation

The countdown is decomposed sequentially into years, months, days, hours, minutes, and seconds using .NET `DateTime` calendar operations. This handles variable month lengths and Gregorian leap years.

The target from `EndDate.txt` is retained as a local civil time for the calendar decomposition, but it is also converted to UTC using `TimeZoneInfo`. Each timer update obtains `DateTime.UtcNow` and uses the UTC values to determine whether the target has actually been reached. This avoids relying on implicit local-time conversions while preserving the intended calendar-style display.

The program does **not** explicitly model leap seconds.

The calculation approach was based on the C# implementation `jwg.cs` from the [date-difference project](https://github.com/jwg4/date-difference).

## Repository layout

```text
Countdown-Clock/
├── .gitignore
├── App.config
├── Countdown_Clock_screencap.png
├── CountdownClock.Designer.vb
├── CountdownClock.resx
├── CountdownClock.sln
├── CountdownClock.vb
├── CountdownClock.vbproj
├── EndDate.txt
├── LICENSE
├── My Project/
├── README.md
└── release/
    └── README.md
```

## License

Countdown Clock is licensed under the **GNU General Public License, version 3 or later**. See [`LICENSE`](LICENSE).
