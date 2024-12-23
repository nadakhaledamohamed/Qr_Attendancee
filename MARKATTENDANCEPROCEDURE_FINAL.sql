
USE [AttendanceSystem];
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

ALTER PROCEDURE [dbo].[MarkAttendance]
    @People_ID INT,
    @Schedule_ID INT,
    @Browser_Data NVARCHAR(255),
    @Start_time DATETIME
AS
BEGIN
  

    DECLARE @TimeInterval INT = 25;
    DECLARE @Role INT;
    DECLARE @Error_Code_ID INT = null;
    DECLARE @Current_Date DATE = CAST(GETDATE() AS DATE);
    DECLARE @Current_Time TIME = CAST(GETDATE() AS TIME);
    DECLARE @Attended_Time DATETIME;
    DECLARE @Lecture_Start DATETIME;
    DECLARE @Lecture_End DATETIME;
    DECLARE @Lecture_Date DATE;
    DECLARE @End_Time DATETIME;

    -- Retrieve the role of the person
    SELECT @Role = Role
    FROM People
    WHERE People_ID = @People_ID;

    -- If the person is a doctor, abort
    IF @Role = 2
    BEGIN
        PRINT 'This procedure is only for students. Operation aborted.';
        RETURN;
    END;

    -- Retrieve lecture details
    SELECT 
        @Lecture_Start = Start_Time,
        @Lecture_End = End_Time,
        @Lecture_Date = [Date]
    FROM Schedule
    WHERE Schedule_ID = @Schedule_ID;

    -- Combine @Start_time, @Attended_Time, and lecture times with the lecture date
   

SET @Attended_Time =  GETDATE();
SET @Lecture_Start =cast(@Lecture_Start as datetime) + cast(@Lecture_Date as datetime)
SET @Lecture_End =cast(@Lecture_End as datetime) + cast(@Lecture_Date as datetime)



SET @End_Time = DATEADD(SECOND, @TimeInterval, @Start_time);    -- Validate if the student is registered for the lecture



--  PRINT 'Start_time: ' + CAST(@Start_time AS NVARCHAR);
--    PRINT 'End_time: ' + CAST(@End_Time AS NVARCHAR);
--    PRINT 'Attended_Time: ' + CAST(@Attended_Time AS NVARCHAR);
--    PRINT 'Lecture_Start: ' + CAST(@Lecture_Start AS NVARCHAR);
--    PRINT 'Lecture_End: ' + CAST(@Lecture_End AS NVARCHAR);

--	IF (@Attended_Time < @Lecture_Start or @Attended_Time < @Lecture_Date)
--    PRINT '@Attended_Time is greater than @End_Time.';

--	if @Attended_Time < @Lecture_Start
--	print '@Attended_Time < @Lecture_Start';
--    ELSE IF @Attended_Time < @End_Time
--    PRINT '@Attended_Time is smaller than @End_Time.'
--else   IF @Attended_Time < @Lecture_Start OR @Attended_Time > @Lecture_End
--	 PRINT 'Error: Attended time is outside lecture start and end time.';





--select @Start_time as qrCode_startTime,@End_Time as qrCode_endTime,@Lecture_Date as Lecture_Date,@Lecture_End as Lecture_End ,@Lecture_Start as Lecture_Start,@Attended_Time as Attended_Time
    

	
 IF 
	NOT EXISTS (
        SELECT 1
        FROM People_Course
        WHERE People_ID = @People_ID
          AND Course_Code_ID = (SELECT Course_Code FROM Schedule WHERE Schedule_ID = @Schedule_ID)
    )
    BEGIN
        SET @Error_Code_ID = 3; -- Not Registered
        PRINT 'Error: Not registered for the lecture.';
        RETURN;
    END;

    -- Validate attendance time
  ELSE  IF (@Attended_Time < @Lecture_Start  OR @Attended_Time > @Lecture_End)

    BEGIN
        SET @Error_Code_ID = 6; -- Outside lecture time
        PRINT 'Error: Attended time is outside lecture start and end time.';
        RETURN;
    END;

     --Check if QR code has expired
   ELSE IF @Attended_Time > @End_Time
    BEGIN
        SET @Error_Code_ID = 2; -- QR code expired
        PRINT 'Error: The QR code has expired.';
        RETURN;
    END;

    -- Insert attendance record
    INSERT INTO Attendance (People_ID, Schedule_ID, Start_Time, End_Time, Status, Browser_Data, Error_Code_ID, Attended_Time)
    VALUES (
        @People_ID,
        @Schedule_ID,
        @Start_time,
        @End_Time,
        CASE WHEN @Error_Code_ID IS NULL THEN 1 ELSE 0 END, -- Status: 1 = Attended, 0 = Not attended
        @Browser_Data,
        @Error_Code_ID,
        @Attended_Time
    );

    PRINT 'Attendance marked successfully.';
END;



DECLARE @Start_time DATETIME = '2024-12-21 10:50:00'; -- Before lecture start time

EXEC [dbo].[MarkAttendance]
    @People_ID = 3,
    @Schedule_ID = 19,
    @Browser_Data = 'Mozilla/5.0',
    @Start_time = @Start_time;