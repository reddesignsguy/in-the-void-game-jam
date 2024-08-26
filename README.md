![image](https://github.com/user-attachments/assets/7caeb7ef-d69a-4800-95fa-faabdbd89680)

### Play the game here: https://vexeo.itch.io/who-is-out-there
--------
## System Design: ![SystemDesigns drawio](https://github.com/user-attachments/assets/a8432da5-581e-453e-b887-1a65a100b403)

----------
### Dev Log
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

### Day 5:
- Level design
- Fixed some bugs with wind current
- Particle wind effect for wind currents which reveal areas that are unaffected by wind using collision detection
- Lighting
- Sprite work
- Parallax effect
- Added some sound FX

https://github.com/user-attachments/assets/c3936885-1724-4591-bb5c-d1739f26e440

### Day 6/7:
- 5 more levels
- Music integration
- More sprites
- Submitted!

### Day 8:
- Added main menu
- Fade transitions between main menu and game
- Camera zoom transition when starting game
- Refactored level resetting
- EXPLOIT FIX: Levels reset any time the player reaches a new checkpoint to prevent blocks from previous levels being carried to newer levels


https://github.com/user-attachments/assets/0be76216-dddb-4979-8057-bb0c2386c078

# Day 9
- Working on polish

TODO:
In The Void
- Character sprite
- Block sprite
    - Middle reveals the direction
- Hovering over block highlights it
- Dragging a block outside the boundary radius doesn’t make you drop the block; It just stays within the boundary and it’s still technically being held.
- Make a radius effect (dashed rotating line) revealing the boundaries
    - If block is at or outside the boundary, the transparency is 100%
    - If within 1m or further inside of the boundary, the transparency starts at 0%
    - If in between the above 2 zones, the transparency transitions 
- Air Boost Dash
