# IN THE VOID -- game jam submission!
## Devlog
### Day 1:
- Created a [sequence diagram](https://lucid.app/lucidchart/ca41642e-a0b1-4818-abf1-f286c3cda8b7/edit?viewport_loc=-1731%2C-975%2C2647%2C2090%2C0_0&invitationId=inv_a9cab8f3-69ea-46f7-8433-a5a2e942a5ea) for setting up the player controls and future system designs
- Created a [game jam doc](https://docs.google.com/document/d/1ImsfMdIiXVEMHOgYnHIrc0tX-SKtv9j9JUVwgk6GeVk/edit?usp=sharing) with ideas for the jam
- Prepared level building assets like collidable rule tiles and imported free 2D unity assets
- Player movement when gravity is on: can only move left and right and jump
- Player movement when gravity is off: can air boost in all directions but no jumping
- Look movement: Player looks at cursor when it moves

![Aug-08-2024 18-20-25](https://github.com/user-attachments/assets/d6b5cb85-1283-4a2c-8da1-973e2693ec4a)

### Day 2:
- Designed sequence diagram for player interaction with objects and UMLs for gates, triggers, and interactable objects
- Set up event-based system for player interaction with objects
- Player can drag and drop objects that are within the player radius
- Gates can be opened and closed by placing or removing "Box Keys" on or from pressure plates
  
https://github.com/user-attachments/assets/9d7e74d4-3c4b-4a7c-97a1-03286b3609f0

### Day 4:
- UI for gravity selection!
- Created wind currents which can blow objects or players away
- If an object is behind another object in a wind current, that object is not affected by the wind current
- Some basic levels created with the help of a "Mechanics matrix" which helped me play around with 2 different mechanics at a time
- Tweaked some physics and some touchups

https://github.com/user-attachments/assets/e3f37cf7-b525-4fe8-9e91-6a0eedfb94b3

