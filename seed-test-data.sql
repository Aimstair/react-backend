-- Insert test users
INSERT INTO dbo.Users (UserId, Password, Name, FirstName, LastName, Email, Role, CreatedBy, CreatedTime, UpdatedBy, UpdatedTime)
VALUES 
('jack', 'asdf1234', 'jack wang', 'jack', 'wang', 'jack@email.com', 'user', 'System', GETDATE(), 'System', GETDATE()),
('admin','asdf1234', 'admin admin', 'admin', 'admin', 'admin@email.com', 'admin', 'System', GETDATE(), 'System', GETDATE()),
('batman', 'asdf1234', 'bruce wayne', 'bruce', 'wayne', 'batman@email.com', 'user', 'System', GETDATE(), 'System', GETDATE()),
('jamal', 'asdf1234', 'jamal johnson', 'jamal', 'johnson', 'jamal@email.com', 'user', 'System', GETDATE(), 'System', GETDATE()),
('chad', 'asdf1234', 'chad wick', 'chad', 'wick', 'chad@email.com', 'user', 'System', GETDATE(), 'System', GETDATE()),
('newjeans', 'asdf1234', 'new jeans', 'new', 'jeans', 'newjeans@email.com', 'user', 'System', GETDATE(), 'System', GETDATE());

-- Insert test rooms
-- Note: Room IDs should match the bookings and room amenities references
INSERT INTO dbo.Rooms (RoomName, FloorNumber, Capacity, CoverPhoto, Available, CreatedBy, CreatedTime, UpdatedBy, UpdatedTime)
VALUES
-- Room ID 2: Board Room (referenced in bookings)
('Board Room', '6th Floor', 20, '/assets/meeting_room.jpg', 1, 'System', GETDATE(), 'System', GETDATE()),

-- Room ID 3: Innovation Hub (referenced in bookings)
('Innovation Hub', '4th Floor', 15, '/assets/meeting_room.jpg', 1, 'System', GETDATE(), 'System', GETDATE()),

-- Room ID 4: Strategy Room (referenced in bookings)
('Strategy Room', '7th Floor', 12, '/assets/meeting_room.jpg', 1, 'System', GETDATE(), 'System', GETDATE()),

-- Room ID 5: Collaboration Space (referenced in bookings)
('Collaboration Space', '7th Floor', 10, '/assets/meeting_room.jpg', 1, 'System', GETDATE(), 'System', GETDATE()),

-- Room ID 6: Executive Suite (referenced in room amenities)
('Executive Suite', '8th Floor', 25, '/assets/meeting_room.jpg', 1, 'System', GETDATE(), 'System', GETDATE()),

-- Room ID 7: Creative Lab (referenced in room amenities)
('Creative Lab', '5th Floor', 8, '/assets/meeting_room.jpg', 1, 'System', GETDATE(), 'System', GETDATE()),

-- Room ID 8: Focus Pod (referenced in room amenities)
('Focus Pod', '3rd Floor', 4, '/assets/meeting_room.jpg', 1, 'System', GETDATE(), 'System', GETDATE()),

-- Room ID 9: Conference Center (referenced in bookings)
('Conference Center', '9th Floor', 50, '/assets/meeting_room.jpg', 1, 'System', GETDATE(), 'System', GETDATE()),

-- Room ID 10: JYP ROOM (referenced in room amenities)
('JYP ROOM', '2nd Floor', 30, '/assets/meeting_room.jpg', 1, 'System', GETDATE(), 'System', GETDATE()),

-- Room ID 11: SM Building (referenced in room amenities)
('SM Building', '1st Floor', 40, '/assets/meeting_room.jpg', 1, 'System', GETDATE(), 'System', GETDATE());

-- Insert test bookings
INSERT INTO dbo.Bookings (RoomId, RoomName, Floor, [Date], StartTime, EndTime, Purpose, Organizer, Recurring, Frequency, RecurringEndDate, DaysOfWeek, Image, CreatedBy, CreatedTime, UpdatedBy, UpdatedTime)
VALUES
(2, 'Board Room', '6th Floor', '2025-11-20', '09:00', '10:00', 'Team Meeting', 'jack', 0, NULL, NULL, NULL, '/assets/meeting_room.jpg', 'System', GETDATE(), 'System', GETDATE()),
(2, 'Board Room', '6th Floor', '2025-11-20', '10:30', '11:30', 'Client Discussion', 'batman', 0, NULL, NULL, NULL, '/assets/meeting_room.jpg', 'System', GETDATE(), 'System', GETDATE()),
(3, 'Innovation Hub', '4th Floor', '2025-11-20', '14:00', '15:00', 'Brainstorming', 'jamal', 0, NULL, NULL, NULL, '/assets/meeting_room.jpg', 'System', GETDATE(), 'System', GETDATE()),
(4, 'Strategy Room', '7th Floor', '2025-11-20', '11:00', '12:00', 'Planning Session', 'chad', 0, NULL, NULL, NULL, '/assets/meeting_room.jpg', 'System', GETDATE(), 'System', GETDATE()),
(9, 'Conference Center', '9th Floor', '2025-11-20', '06:00', '18:00', 'Conference', 'jack', 1, 'Weekly', '2025-12-31', 'Monday,Wednesday,Friday', '/assets/meeting_room.jpg', 'System', GETDATE(), 'System', GETDATE()),
(5, 'Collaboration Space', '7th Floor', '2025-11-20', '13:00', '14:00', 'Project Review', 'newjeans', 0, NULL, NULL, NULL, '/assets/meeting_room.jpg', 'System', GETDATE(), 'System', GETDATE()),
(2, 'Board Room', '6th Floor', '2025-11-21', '09:00', '10:00', 'Follow-up Meeting', 'batman', 0, NULL, NULL, NULL, '/assets/meeting_room.jpg', 'System', GETDATE(), 'System', GETDATE()),
(3, 'Innovation Hub', '4th Floor', '2025-11-21', '15:00', '16:00', 'Workshop', 'jack', 0, NULL, NULL, NULL, '/assets/meeting_room.jpg', 'System', GETDATE(), 'System', GETDATE());

-- Insert booking participants
INSERT INTO dbo.BookingParticipants (BookingId, UserId)
VALUES
(1, 'batman'),
(1, 'jamal'),
(2, 'jack'),
(2, 'chad'),
(3, 'jack'),
(3, 'newjeans'),
(4, 'batman'),
(5, 'jack'),
(5, 'batman'),
(5, 'jamal'),
(6, 'chad'),
(6, 'newjeans'),
(7, 'jamal'),
(8, 'batman'),
(8, 'newjeans');

-- Insert test amenities
INSERT INTO dbo.Amenities (Name, Description, IsActive, CreatedBy, CreatedTime)
VALUES
('Projector', 'High-definition projector for presentations', 1, 'System', GETDATE()),
('Whiteboard', 'Large interactive whiteboard', 1, 'System', GETDATE()),
('Conference Phone', 'Speakerphone for conference calls', 1, 'System', GETDATE()),
('Video Conference System', 'HD video conferencing setup', 1, 'System', GETDATE()),
('WiFi', 'High-speed WiFi connectivity', 1, 'System', GETDATE()),
('Catering Service', 'In-room catering available', 1, 'System', GETDATE()),
('Air Conditioning', 'Climate control system', 1, 'System', GETDATE()),
('Parking', 'Dedicated parking spaces', 1, 'System', GETDATE());

-- Link amenities to rooms (RoomId to AmenityId)
INSERT INTO dbo.RoomAmenities (RoomId, AmenityId)
VALUES
-- Board Room (RoomId 2)
(2, 1), (2, 2), (2, 3), (2, 4), (2, 5),
-- Innovation Hub (RoomId 3)
(3, 1), (3, 2), (3, 4), (3, 5), (3, 6),
-- Strategy Room (RoomId 4)
(4, 1), (4, 2), (4, 3), (4, 5), (4, 7),
-- Collaboration Space (RoomId 5)
(5, 1), (5, 2), (5, 5), (5, 6), (5, 7),
-- Executive Suite (RoomId 6)
(6, 1), (6, 2), (6, 3), (6, 4), (6, 5), (6, 6), (6, 7), (6, 8),
-- Creative Lab (RoomId 7)
(7, 1), (7, 2), (7, 5), (7, 6), (7, 7),
-- Focus Pod (RoomId 8)
(8, 5), (8, 7),
-- Conference Center (RoomId 9)
(9, 1), (9, 2), (9, 3), (9, 4), (9, 5), (9, 6), (9, 7), (9, 8),
-- JYP ROOM (RoomId 10)
(10, 1), (10, 2), (10, 5), (10, 7),
-- SM Building (RoomId 11)
(11, 1), (11, 2), (11, 3), (11, 5), (11, 8);

