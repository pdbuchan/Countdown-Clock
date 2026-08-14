' Countdown Clock Copyright 2017-2026 Paul David Buchan (pdbuchan@gmail.com)
' SPDX-License-Identifier: GPL-3.0-or-later
'
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
' GNU General Public License for more details.
'
Imports System.Globalization

Public Class CountdownClock
    Dim EndDateLocal As DateTime
    Dim EndDateUtc As DateTime

    Private Sub CountdownClock_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim InputFileName, stringReader, DateString, Format As String
        Dim fileReader As System.IO.StreamReader
        Dim StringArray As String()
        InputFileName = "EndDate.txt"

        'Open file containing end-date and windows form title bar text.
        Try
            fileReader = My.Computer.FileSystem.OpenTextFileReader(InputFileName)
        Catch exc As System.IO.FileNotFoundException
            MessageBox.Show("Can't find file " & InputFileName & ".", "File not found.", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End
        Catch exc As Exception
            MessageBox.Show(exc.Message)
            End
        End Try

        'Read line of text from input file.
        stringReader = fileReader.ReadLine()

        'Parse end-date and window form title bar text from stringReader.
        StringArray = stringReader.Split(New Char() {","})
        DateString = StringArray(0)
        Me.Text = StringArray(1)

        'Close end-date file.
        fileReader.Close()

        'Convert end date & time string to type DateTime. The value in EndDate.txt
        'is a local wall-clock time, so keep its DateTimeKind as Unspecified until
        'it is explicitly converted using the computer's local time-zone rules.
        Format = "yyyy-MM-dd HH:mm:ss"
        If Not DateTime.TryParseExact(DateString, Format, CultureInfo.InvariantCulture,
                                      DateTimeStyles.None, EndDateLocal) Then
            MessageBox.Show("Unable to understand date and time from input file. " &
                            "Format should be yyyy-MM-dd HH:mm:ss,message",
                              DateString, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End
        End If
        EndDateLocal = DateTime.SpecifyKind(EndDateLocal, DateTimeKind.Unspecified)

        'A local time can be invalid when the clock moves forward for daylight
        'saving time, or ambiguous when the clock moves backward and an hour occurs
        'twice. Refuse either case rather than silently choosing the wrong instant.
        If TimeZoneInfo.Local.IsInvalidTime(EndDateLocal) Then
            MessageBox.Show("The end date and time does not exist in the local time zone " &
                            "because of a daylight-saving time transition. Please choose " &
                            "a different time.",
                            DateString, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End
        End If
        If TimeZoneInfo.Local.IsAmbiguousTime(EndDateLocal) Then
            MessageBox.Show("The end date and time occurs twice in the local time zone " &
                            "because of a daylight-saving time transition. Please choose " &
                            "an unambiguous time.",
                            DateString, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End
        End If

        'Convert the local target to an absolute UTC instant. UTC is used to decide
        'whether the target has actually been reached, avoiding local-time ambiguity.
        EndDateUtc = TimeZoneInfo.ConvertTimeToUtc(EndDateLocal, TimeZoneInfo.Local)

        'Start timer so countdown can start.
        Timer.Start()
    End Sub

    Private Sub Timer_Tick(sender As Object, e As EventArgs) Handles Timer.Tick
        Dim NowUtc, StartDate, d As DateTime
        Dim Years, Months, Days, Hours, Minutes, Seconds As Integer

        'Obtain the current instant in UTC. This is the authoritative clock used to
        'determine whether the target has been reached.
        NowUtc = DateTime.UtcNow

        'If end-date and time has been reached, stop timer.
        If EndDateUtc <= NowUtc Then
            Timer.Stop()
            Return
        End If

        'Convert the current instant to local wall-clock time for calendar arithmetic.
        'Use DateTimeKind.Unspecified so all calculations below are explicitly between
        'local civil times rather than relying on implicit DateTime conversions.
        StartDate = TimeZoneInfo.ConvertTimeFromUtc(NowUtc, TimeZoneInfo.Local)
        StartDate = DateTime.SpecifyKind(StartDate, DateTimeKind.Unspecified)

        'Calculate number of years to end-date.
        Years = 0
        d = StartDate
        While (d <= EndDateLocal)
            Years += 1
            d = StartDate.AddYears(Years)
        End While
        Years -= 1
        StartDate = StartDate.AddYears(Years)
        Label1.Text = Years.ToString("00")

        'Calculate number of months to end-date, exclusive of years from above.
        Months = 0
        d = StartDate
        While (d <= EndDateLocal)
            Months += 1
            d = StartDate.AddMonths(Months)
        End While
        Months -= 1
        StartDate = StartDate.AddMonths(Months)
        Label2.Text = Months.ToString("00")

        'Calculate number of days to end-date, exclusive of years and months from above.
        Days = 0
        d = StartDate
        While (d <= EndDateLocal)
            Days += 1
            d = StartDate.AddDays(Days)
        End While
        Days -= 1
        StartDate = StartDate.AddDays(Days)
        Label3.Text = Days.ToString("00")

        'Calculate number of hours to end-date, exclusive of years, months, and days from above.
        Hours = 0
        d = StartDate
        While (d <= EndDateLocal)
            Hours += 1
            d = StartDate.AddHours(Hours)
        End While
        Hours -= 1
        StartDate = StartDate.AddHours(Hours)
        Label4.Text = Hours.ToString("00")

        'Calculate number of minutes to end-date, exclusive of years, months, days, and hours from above.
        Minutes = 0
        d = StartDate
        While (d <= EndDateLocal)
            Minutes += 1
            d = StartDate.AddMinutes(Minutes)
        End While
        Minutes -= 1
        StartDate = StartDate.AddMinutes(Minutes)
        Label5.Text = Minutes.ToString("00")

        'Calculate number of seconds to end-date, exclusive of years, months, days, hours, and minutes from above.
        Seconds = 0
        d = StartDate
        While (d <= EndDateLocal)
            Seconds += 1
            d = StartDate.AddSeconds(Seconds)
        End While
        Seconds -= 1
        StartDate = StartDate.AddSeconds(Seconds)
        Label6.Text = Seconds.ToString("00")
    End Sub
End Class
