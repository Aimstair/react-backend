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

