Urban Planning Simulation

An advanced urban development simulator that lets urban planners and city developers test infrastructure changes, population dynamics, and environmental impacts in a real-time 3D environment. Features SimCore (Main System)

Controls the main simulation loop
Manages save/load operations
Coordinates between other systems
Handles basic time controls (play/pause)

Population System

Manages up to 100 citizens
Controls basic behaviors (home/work cycles)
Handles simple pathfinding
Tracks basic needs (housing, jobs)

City Grid

Maintains a 5x5 mile city grid
Handles three zone types
Controls basic building placement
Manages land usage

Basic Analytics

Tracks essential city statistics
Generates simple reports
Monitors basic population data
Creates CSV exports

Getting Started

Clone the repository
Open in Unity 2022.3 LTS
Install required packages
Run the starter scene

Prerequisites

Unity 2022.3 LTS
Visual Studio 2022
Git

Development Tools

Git for version control
GitHub for code hosting
Visual Studio 2022
Discord for team chat

Project Structure Graham - Unity Setup & Grid

Tools:

Unity 2022.3 LTS
Basic Unity UI
ProBuilder (for simple shapes)
Standard Unity components

Danny - Population & Movement

Tools:

Unity NavMesh
Basic Animation system
Simple Unity Physics
Unity UI elements

Reid/Karlene - Backend & Analytics

Tools:

SQLite (basic setup)
C++
Simple JSON handling
CSV export tools

Implementation Timeline Week 1: Foundation

Set up Unity project
Create basic grid system
Implement simple UI
Set up database

Week 2: Core Systems

Add zone placement
Create citizen movement
Implement save/load
Add basic statistics

Week 3: Integration

Connect all systems
Add basic animations
Implement reporting
Create visualization

Week 4: Polish

Bug fixing
Basic optimization
Add simple tutorials
Final testing

System Interactions Core Interactions

SimCore ↔ Grid
    Updates zone changes
    Manages building placement
    Handles save/load operations
SimCore ↔ Population
    Updates citizen positions
    Manages behavior states
    Controls spawning/despawning
Analytics ↔ Other Systems
    Collects basic stats
    Tracks population data
    Monitors zone usage

Contributing
Karlene Bienaime
Reid Blankenship
Graham Bergeron
Danny Bui
