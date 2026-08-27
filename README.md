# TrackIR: 3D Asteroids Game

**TrackIR: 3D Asteroids Game is an immersive space arcade game designed to demonstrate how head-tracking controls can create a more engaging, hands-free gameplay experience for trade shows, demos, and interactive technology showcases.**

Players pilot a spaceship through a 3D asteroid field using TrackIR head-tracking input and a single button to fire lasers. The game reimagines the classic Asteroids formula as a modern 3D demo that highlights how OptiTrack’s TrackIR technology can be used beyond traditional simulation environments.

---

## Why This Project Matters

Interactive hardware demos are most effective when people can immediately understand what the technology does and why it feels different. Traditional software demos can be difficult to explain quickly, especially in a trade show environment where users may only have a few minutes to engage.

This project gives TrackIR a clear, playable demonstration: users can control a spaceship with natural head movement, avoid incoming asteroids, fire lasers, and experience responsive gameplay without needing an entire keyboard, mouse, or complex controller setup.

The result is a lightweight arcade experience that makes head-tracking technology easy to see, easy to try, and easy to remember.

---

## Project Highlights

### Head-Tracking Spaceflight

Players control the spaceship using TrackIR head movement, allowing the game to demonstrate responsive head-tracking input in a way that feels natural and immediately understandable.

<!-- ![Spaceship flying through asteroid field]() picture demonstrating this point -->
<img width="852" height="477" alt="An image of a spaceship flying through an asteroid field." src="https://github.com/user-attachments/assets/ec5fc9a3-ac3b-4a8f-8401-ccae1f38df72" />

---

### One-Button Arcade Gameplay

The game is designed to be playable with TrackIR and a single button. This makes it easier for new users to try the demo quickly without learning a complicated control scheme.

<!-- ![Player firing lasers at asteroids]() picture demonstrating this point-->
<img width="852" height="477" alt="An image of a spaceship firing lasers at asteroids." src="https://github.com/user-attachments/assets/8a6242cd-6a67-4a7e-9003-0c56ea487b63" />

---

### Dynamic Asteroid Combat

Asteroids move through the 3D space environment, collide with the player, and can be destroyed with lasers. Larger asteroids split into smaller pieces, creating a more active and replayable arcade loop.

<!-- ![Asteroid splitting after being hit]() picture demonstrating this -->
<img width="852" height="477" alt="An image of an asteroid splitting in two." src="https://github.com/user-attachments/assets/ed7b2b8a-479a-4830-b932-9662ba9a4735" />

---

### Trade Show Mode

The project includes a trade show-focused gameplay mode intended for quick public demonstrations. The experience is designed to be accessible, visually clear, and easy to restart for repeated users.

<!-- ![Trade show mode menu](docs/images/trade-show-mode.png) picture demonstrating this??? -->

---

### Polished Visual Interface

The game includes a custom neon space-themed interface, including menus, HUD elements, score display, hull/health status, and visual feedback designed to make the demo feel like a finished product.

<!-- ![Main menu and HUD interface]() picture demonstrating this??? -->
<img width="852" height="477" alt="An image of the game's main menu." src="https://github.com/user-attachments/assets/9541a98d-6c88-4055-826a-50df4f0e1042" />

---

## Intended Audience

This project is intended for:

- OptiTrack / NaturalPoint demo environments
- Trade show visitors
- Users interested in head-tracking interaction
- Developers exploring alternative input systems

---

## Current Status

The project is currently under active development as part of the **Oregon State University Senior Software Engineering Capstone** sequence.

Current work focuses on:

- Improving the feel and reliability of TrackIR-based gameplay
- Refining asteroid spawning, movement, collision, and splitting behavior
- Polishing menus, HUD elements, and score display
- Testing the game in builds, not only inside the Unity Editor
- Preparing the project for public demonstration, partner feedback, and portfolio use

Because the project is still in development, some features, setup steps, and demo materials may continue to change as the team receives feedback and improves the experience.

---

## Demo

[Latest Release](https://github.com/TrackIR/Game-Flight-Shooter/releases)

**View the project on GitHub:**  
[TrackIR: 3D Asteroids Game Repository](https://github.com/TrackIR/Game-Flight-Shooter)

**Watch the gameplay demo:**

A public video demo will be linked here when available.

---

## How to Access or Run the Project

This project is built in **Unity** and uses **TrackIR** hardware for the intended gameplay experience.

### Running the Project as an Executable

#### Requirements

- [TrackIR hardware and TrackIR software](https://www.trackir.com)
- Keyboard or button input for firing/selecting actions
- Windows PC recommended

#### Steps
1. Set up the TrackIR hardware and start the TrackIR app
   
2. Download the public demo above
   
3. Unzip the folder
   
4. Run the .exe file titled `AsteroidsIR.exe`

### Opening the Project in Unity

#### Requirements

- Unity Editor
- [TrackIR hardware and TrackIR software](https://www.trackir.com/)
- Windows PC recommended
- Keyboard or button input for firing/selecting actions
- Repository access through GitHub

### Steps

1. Clone the repository:

   ```sh
   git clone https://github.com/TrackIR/Game-Flight-Shooter
   ```

2. Open Unity Hub and navigate to Projects

3. Click `Add` -> `Add project from disk`

4. Navigate to the cloned repository and `Open` it in the file explorer

5. Open the project from the Unity Hub project's menu

6. Set up the TrackIR hardware and start the TrackIR app

7. Run the game in the editor or make changes

---

## Team

This project was developed by **Team 30** as part of the **Oregon State University Senior Software Engineering Capstone** program.

The team collaborated across gameplay systems, TrackIR integration, UI polish, testing, documentation, and project planning.

| Team Member     | GitHub                                               | Contact                  |
| --------------- | ---------------------------------------------------- | ------------------------ |
| Samuel Cardwell | [@SamuelCardwell](https://github.com/SamuelCardwell) | cardwels@oregonstate.edu |
| Bryan Partida   | [@bryanpartida](https://github.com/bryanpartida)     | partidab@oregonstate.edu |
| Hunter Simpson  | [@itsautummn](https://github.com/itsautummn)         | simpsohu@oregonstate.edu |
| Jakob Jensen    | [@jensjako](https://github.com/jensjako)             | jensjako@oregonstate.edu |
| Leo Yudelson    | [@yudelsol](https://github.com/yudelsol)             | yudelsol@oregonstate.edu |

---

## Project Partner

This project was developed in partnership with **NaturalPoint Inc. DBA OptiTrack** as part of the **Oregon State University Senior Software Engineering Capstone** program.

OptiTrack/NaturalPoint provided the project direction and context for a TrackIR-based interactive demo that can help showcase head-tracking technology in a clear, engaging, and memorable way.

### Partner Contacts

| Name            | GitHub                                   |
| --------------- | ---------------------------------------- |
| Ryan Mann       | [@mannry22](https://github.com/mannry22) |
| Anthony Lazzaro | [@skynab](https://github.com/skynab)     |

## License

All non-third-party code included in this repository is jointly owned by the team listed above and **NaturalPoint Inc. DBA OptiTrack**.

See `LICENSE.txt` in the `licenses` folder for more information.

Third-party licenses can also be found in the `licenses` folder.
