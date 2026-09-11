# 🏋️ Workout Plan Feature - Frontend Implementation Guide

---

## 1. OVERVIEW

### Feature Requirements

- ✅ Display member's active workout plan with all details
- ✅ Show exercise images/videos during workout
- ✅ Track workout completion and progress
- ✅ Regenerate workout plan (new exercises)
- ✅ Integrated with existing member layout and dashboard

### Backend Integration Details

**API Endpoints:**

- `GET /api/workout-plans/active` - Get current active plan
- `POST /api/workout-plans/generate` - Generate new plan (archives current)

**Split Types Available:**

- `FULL_BODY` - 3 full-body sessions
- `UPPER_LOWER` - 4 days (Upper/Lower alternating)
- `BRO_SPLIT_4DAY` - 4 days (Chest+Tri, Back+Bi, Shoulders, Legs)
- `BRO_SPLIT_5DAY` - 5 days (Chest, Back, Shoulders, Legs, Arms)
- `PUSH_PULL_LEGS` - 6 days (Push/Pull/Legs repeated)

**Fitness Goals:**

- `MUSCLE_GAIN` - Hypertrophy focus
- `WEIGHT_LOSS` - Cardio/calorie burn focus
- `ENDURANCE` - Higher reps, circuit training
- `GENERAL_FITNESS` - Balanced approach

**Difficulty Levels:**

- `BEGINNER` - 3 sets, moderate reps
- `INTERMEDIATE` - 4 sets, moderate reps
- `ADVANCED` - 5 sets, higher intensity

**Exercise Selection Logic:**

- 2:1 ratio of compound to isolation exercises
- Exercises selected based on muscle groups and difficulty
- Prevents duplicate exercises within a plan
- Includes equipment requirements and muscle targets

### User Flow

```
Member Dashboard
  ↓ (Click "Workout Plan" / "Start Workout")
Member Workout Plan Page
  ├─→ View Full Plan (days, exercises, details)
  ├─→ Start/Continue Workout (with progress tracking)
  └─→ Regenerate Plan (archive current, create new)
```

---

## 2. FILE STRUCTURE & ARCHITECTURE

### 2.1 New Files to Create

```
src/
├── types/
│   └── workoutPlan.ts                    ⭐ Data types for workout plan
│
├── services/
│   └── workoutPlanService.ts            ⭐ API calls & data fetching
│
├── hooks/
│   └── useWorkoutPlan.ts                ⭐ Custom hook for plan management
│   └── useWorkoutProgress.ts            ⭐ Track workout session progress
│
├── components/
│   └── workout/                         ⭐ NEW FOLDER
│       ├── WorkoutPlanCard.tsx          - Plan overview card
│       ├── WorkoutDayTab.tsx            - Tab for each day
│       ├── ExerciseCard.tsx             - Individual exercise display
│       ├── ExerciseMediaViewer.tsx      - Images/videos modal
│       ├── WorkoutProgressTracker.tsx   - Current workout progress
│       └── RegenerateModal.tsx          - Confirm regenerate dialog
│
└── pages/
    └── MemberWorkoutPlan.tsx            ⭐ Main page component

```

### 2.2 Modified Files

```
src/
├── router/index.tsx                     - Add route for workout plan page
├── layouts/MemberLayout.tsx             - Already has link (no change)
└── pages/MemberDashboard.tsx            - Already has link (no change)
```

---

## 3. DATA TYPES (workoutPlan.ts)

```typescript
// Main Plan Response from Backend
interface WorkoutPlan {
  id: number;
  name: string;
  splitType: SplitType;
  goal: FitnessGoal;
  level: FitnessLevel;
  status: "ACTIVE" | "ARCHIVED";
  createdAt: string; // ISO date
  days: WorkoutDay[];
}

interface WorkoutDay {
  dayNumber: number;
  muscleGroupLabel: string; // e.g., "Chest & Triceps", "Upper Body", "Push"
  exercises: WorkoutExercise[];
}

interface WorkoutExercise {
  id: number;
  exerciseName: string; // From exercise translations
  exerciseCode: string; // 10-char code
  difficulty: DifficultyLevel;
  sets: number; // 3-5 based on level
  reps: number; // Min reps (e.g., 8)
  repsMax: number; // Max reps (e.g., 12)
  durationSeconds?: number; // For cardio exercises
  targetMuscles: string[]; // e.g., ["Chest", "Triceps"]
  primaryMuscle: string; // Main target muscle
  equipment: string[]; // e.g., ["Dumbbell", "Barbell"]
  images: MediaFile[]; // Exercise images
  videos: MediaFile[]; // Exercise videos/GIFs
  orderIndex: number; // Exercise order within day
}

interface MediaFile {
  id: string;
  url: string;
  type: "IMAGE" | "VIDEO" | "GIF";
  description?: string;
}

// Enums from Backend
type SplitType =
  | "FULL_BODY" // 3 days, full body each session
  | "UPPER_LOWER" // 4 days, Upper/Lower alternating
  | "BRO_SPLIT_4DAY" // 4 days: Chest+Tri, Back+Bi, Shoulders, Legs
  | "BRO_SPLIT_5DAY" // 5 days: Chest, Back, Shoulders, Legs, Arms
  | "PUSH_PULL_LEGS"; // 6 days: Push, Pull, Legs (repeated)

type FitnessGoal =
  | "MUSCLE_GAIN" // Hypertrophy: 8-12 reps, 4 sets
  | "WEIGHT_LOSS" // Cardio focus: higher reps, circuits
  | "ENDURANCE" // Higher reps, circuit training
  | "GENERAL_FITNESS"; // Balanced approach

type FitnessLevel =
  | "BEGINNER" // 3 sets, moderate intensity
  | "INTERMEDIATE" // 4 sets, moderate intensity
  | "ADVANCED"; // 5 sets, higher intensity

type DifficultyLevel =
  | "EASY" // Beginner exercises
  | "MEDIUM" // Intermediate exercises
  | "HARD"; // Advanced exercises

// Progress Tracking (Local State / LocalStorage)
interface WorkoutProgress {
  planId: number;
  currentDayNumber: number;
  currentExerciseIndex: number;
  completedExercises: ExerciseProgress[];
  workoutStartTime: string; // ISO date
  workoutEndTime?: string;
  status: "IN_PROGRESS" | "COMPLETED" | "PAUSED";
}

interface ExerciseProgress {
  exerciseId: number;
  completedSets: SetProgress[];
  notes?: string;
  completed: boolean;
  completedAt?: string;
}

interface SetProgress {
  setNumber: number;
  repsCompleted: number;
  weight?: number;
  notes?: string;
}
```

---

## 4. SERVICE LAYER (workoutPlanService.ts)

```typescript
// API Calls
export const getActivePlan = async (): Promise<WorkoutPlan>
  // GET /api/workout-plans/active
  // Returns: WorkoutPlan or 404 if no active plan

export const generateNewPlan = async (): Promise<WorkoutPlan>
  // POST /api/workout-plans/generate
  // Generates new plan, archives current one
  // Returns: New WorkoutPlan

// Local Storage (Progress Tracking)
export const saveWorkoutProgress = (progress: WorkoutProgress): void
  // Key: 'workout_progress_<planId>'

export const loadWorkoutProgress = (planId: number): WorkoutProgress | null
  // Retrieves progress for plan

export const clearWorkoutProgress = (planId: number): void
  // Clears progress (after completed/abandoned)

export const getProgressPercentage = (planId: number): number
  // Calculate % of exercises completed
```

---

## 5. CUSTOM HOOKS

### 5.1 useWorkoutPlan.ts

```typescript
interface UseWorkoutPlanReturn {
  plan: WorkoutPlan | null;
  loading: boolean;
  error: string | null;
  generateNewPlan: () => Promise<void>;
  refetch: () => Promise<void>;
}

// Usage:
const { plan, loading, error, generateNewPlan, refetch } = useWorkoutPlan();
```

**Logic:**

- Fetch active plan on mount
- Handle errors gracefully
- Provide refresh function
- Generate new plan with confirmation

### 5.2 useWorkoutProgress.ts

```typescript
interface UseWorkoutProgressReturn {
  progress: WorkoutProgress | null;
  currentDay: WorkoutDay | null;
  currentExercise: WorkoutExercise | null;
  progressPercent: number;
  completeExercise: (exerciseId: number, setsData: SetProgress[]) => void;
  moveToNextExercise: () => void;
  startWorkout: (planId: number, dayNumber: number) => void;
  completeWorkout: () => void;
  resetProgress: () => void;
}

// Usage:
const {
  progress,
  currentExercise,
  progressPercent,
  completeExercise,
  moveToNextExercise,
} = useWorkoutProgress();
```

**Logic:**

- Load progress from localStorage
- Track current day/exercise
- Mark exercises as complete
- Calculate overall progress
- Handle workout completion

---

## 6. COMPONENT ARCHITECTURE

### 6.1 MemberWorkoutPlan.tsx (Main Page)

**Layout:**

```
┌─────────────────────────────────────────┐
│ HEADER                                   │
│ "Your Workout Plan" + Plan Info          │
└─────────────────────────────────────────┘

┌─────────────────────────────────────────┐
│ PLAN OVERVIEW CARD                      │
│ Split: PPL | Goal: Muscle Gain          │
│ Level: Advanced | Created: 2 days ago    │
│ [Generate New Plan] [Start Workout]      │
└─────────────────────────────────────────┘

┌─────────────────────────────────────────┐
│ PROGRESS INDICATOR (if in progress)     │
│ Day 3 of 6 | 15/24 exercises done        │
│ ████████░░ 62%                           │
└─────────────────────────────────────────┘

┌─────────────────────────────────────────┐
│ DAY TABS / ACCORDION                    │
│ [Day 1: Chest] [Day 2: Back] [Day 3:...] │
│ ┌───────────────────────────────────────┤
│ │ Exercise 1: Bench Press               │
│ │ 4 sets × 8-10 reps | [View Images] ✓  │
│ │ Equipment: Barbell                    │
│ │ Muscles: Chest, Triceps               │
│ └───────────────────────────────────────┤
│ │ Exercise 2: Incline Dumbbell Press    │
│ │ 3 sets × 10-12 reps | [View Images]   │
│ └───────────────────────────────────────┤
└─────────────────────────────────────────┘
```

**Features:**

- Plan summary at top
- Tabs/Accordion for each day
- Each day shows all exercises
- Click exercise card → Open media viewer
- CTA buttons for regenerate & start

---

### 6.2 WorkoutPlanCard.tsx

**Props:**

```typescript
interface Props {
  plan: WorkoutPlan;
  progressPercent?: number;
  onStartWorkout: () => void;
  onRegenerateClick: () => void;
  isLoading?: boolean;
}
```

**Display:**

- Plan name
- Split type badge (visual)
- Goal & Level
- Created date
- Progress bar (if applicable)
- Action buttons

---

### 6.3 WorkoutDayTab.tsx

**Props:**

```typescript
interface Props {
  day: WorkoutDay;
  exercises: WorkoutExercise[];
  isCurrentDay?: boolean;
  onExerciseClick: (exercise: WorkoutExercise) => void;
  completedExerciseIds?: number[];
}
```

**Display:**

- Day number + muscle group label (e.g., "Day 1: Push")
- List of exercises in order
- Checkmark if completed
- Exercise details (sets × reps, muscles, equipment)

---

### 6.4 ExerciseCard.tsx

**Props:**

```typescript
interface Props {
  exercise: WorkoutExercise;
  isCompleted?: boolean;
  orderIndex?: number;
  onViewMedia: () => void;
  onMarkComplete?: () => void;
}
```

**Display:**

- Exercise name
- Sets × Reps
- Primary muscle group (colored badge)
- Secondary muscles
- Equipment needed
- Thumbnail of first image
- Buttons: [View Images] [View Details]
- Checkmark if completed

---

### 6.5 ExerciseMediaViewer.tsx (Modal)

**Props:**

```typescript
interface Props {
  exercise: WorkoutExercise;
  isOpen: boolean;
  onClose: () => void;
  onMarkComplete?: () => void;
}
```

**Display:**

```
┌──────────────────────────────────────┐
│ X Close                              │
├──────────────────────────────────────┤
│ BENCH PRESS                          │
│ Muscles: Chest, Triceps              │
│ Equipment: Barbell                   │
├──────────────────────────────────────┤
│                                      │
│  [Image 1] [Image 2] [Image 3]      │
│  ◄ Main Image ►                      │
│  Large display area                  │
│                                      │
├──────────────────────────────────────┤
│ 4 sets × 8-10 reps                   │
│ ☐ Mark as Complete                   │
│ [Next Exercise] [Done]               │
└──────────────────────────────────────┘
```

**Features:**

- Image gallery with thumbnails
- Video playback if available
- Exercise details
- "Mark Complete" checkbox
- Navigation to next exercise

---

### 6.6 WorkoutProgressTracker.tsx

**Props:**

```typescript
interface Props {
  progress: WorkoutProgress;
  plan: WorkoutPlan;
  onExerciseComplete: (exerciseId: number, setsData: SetProgress[]) => void;
  onNextExercise: () => void;
}
```

**Display:**

```
┌──────────────────────────────────────┐
│ CURRENT WORKOUT                      │
│ Day 3 of 6 | Rest Days: 1            │
├──────────────────────────────────────┤
│ Exercise 5/8: Leg Press              │
│ 4 sets × 8-10 reps                   │
├──────────────────────────────────────┤
│ Set 1: [___] reps [___] lbs          │
│ Set 2: [___] reps [___] lbs          │
│ Set 3: [___] reps [___] lbs          │
│ Set 4: [___] reps [___] lbs          │
│ Notes: [________________]            │
├──────────────────────────────────────┤
│ [View Exercise] [Mark Complete] [..] │
└──────────────────────────────────────┘
```

**Features:**

- Current exercise & day info
- Editable sets/reps tracker
- Notes field
- Buttons: View media, mark complete, skip, rest day

---

### 6.7 RegenerateModal.tsx

**Props:**

```typescript
interface Props {
  isOpen: boolean;
  isLoading: boolean;
  onConfirm: () => Promise<void>;
  onCancel: () => void;
}
```

**Display:**

```
┌──────────────────────────────────────┐
│ Generate New Workout Plan?           │
├──────────────────────────────────────┤
│ Your current plan will be archived.  │
│ You'll get new exercises with the    │
│ same goals and intensity.            │
├──────────────────────────────────────┤
│ [Cancel] [Generate]                  │
└──────────────────────────────────────┘
```

---

## 7. STATE MANAGEMENT

### Global State (Not needed - simple API calls)

- Use hooks for local component state
- Use localStorage for progress tracking
- Use React Query / SWR for API caching (optional)

### Local State (Per Component)

**MemberWorkoutPlan:**

- `plan` - Current workout plan
- `loading` - Fetch loading state
- `error` - Error message
- `selectedDayIndex` - Active tab
- `regenerateModalOpen` - Show regenerate confirmation
- `mediaViewerOpen` - Show media viewer modal
- `selectedExercise` - Current exercise for media viewer

**WorkoutProgressTracker:**

- `progress` - Workout session progress
- `currentSetData` - Input for current set
- `isTracking` - Is workout in progress

---

## 8. DATA FLOW DIAGRAM

```
┌──────────────────────┐
│ MemberWorkoutPlan    │
│ (Main Page)          │
└──────────┬───────────┘
           │
     ┌─────▼──────────────────────────────┐
     │ useWorkoutPlan()                   │
     │ ├─ getActivePlan()  ──┐            │
     │ └─ generateNewPlan()  ├─→ API      │
     │                        │            │
     │    Provides: plan,     │            │
     │    loading, error      │            │
     └─────┬──────────────────┘            │
           │                               │
     ┌─────▼────────────────┐             │
     │ Plan Display Section │             │
     │ ├─ WorkoutPlanCard   │             │
     │ └─ WorkoutDayTab ────┼──┐          │
     │    ├─ ExerciseCard ──┼──┼─┐        │
     │    └─ [Click] ───────┘  │ │        │
     └────────────────────────┼─┼────────┘
                              │ │
                       ┌──────▼─▼──────┐
                       │ ExerciseMedia  │
                       │ Viewer Modal   │
                       └────────────────┘

┌──────────────────────────────────────────┐
│ Workout Session (Progress Tracking)      │
├──────────────────────────────────────────┤
│ useWorkoutProgress()                     │
│ ├─ loadProgress from localStorage        │
│ ├─ Progress state management             │
│ └─ Provides hooks for tracking           │
│                                          │
│ WorkoutProgressTracker Component         │
│ ├─ Display current exercise              │
│ ├─ Input set/rep data                    │
│ ├─ Mark exercise complete                │
│ └─ Save to localStorage                  │
└──────────────────────────────────────────┘

┌──────────────────────────────────────────┐
│ Regenerate Plan                          │
├──────────────────────────────────────────┤
│ [Generate New Plan] clicked              │
│     ↓                                    │
│ RegenerateModal opened                   │
│     ↓                                    │
│ User confirms                            │
│     ↓                                    │
│ generateNewPlan() API call               │
│     ↓                                    │
│ Update UI with new plan                  │
│ Clear old progress from localStorage     │
└──────────────────────────────────────────┘
```

---

## 9. API INTEGRATION

### Backend Architecture Summary

**Database Structure:**

- `WorkoutPlan` → `WorkoutDay` → `WorkoutExercise` → `Exercise`
- Plans are member-specific and auto-generated based on profile
- Exercise selection uses 2:1 compound:isolation ratio
- Plans include full exercise details with media and equipment

**Business Logic:**

- **Sets by Level:** Beginner=3, Intermediate=4, Advanced=5
- **Reps by Goal:** Strength=4-6, Hypertrophy=8-12, Circuit=15-20
- **Split Logic:** Defines muscle groups worked each day
- **Exercise Selection:** Prevents duplicates, matches difficulty level

### Endpoints Used

**1. Get Active Plan**

```
GET /api/workout-plans/active
Authorization: Bearer {token}

Response (200):
{
  "id": 1,
  "name": "Push Pull Legs - Muscle Gain",
  "splitType": "PUSH_PULL_LEGS",
  "goal": "MUSCLE_GAIN",
  "level": "INTERMEDIATE",
  "status": "ACTIVE",
  "createdAt": "2024-04-19T10:00:00Z",
  "days": [
    {
      "dayNumber": 1,
      "muscleGroupLabel": "Push",
      "exercises": [
        {
          "id": 123,
          "exerciseName": "Bench Press",
          "exerciseCode": "BP001",
          "difficulty": "MEDIUM",
          "sets": 4,
          "reps": 8,
          "repsMax": 12,
          "targetMuscles": ["Chest", "Triceps"],
          "primaryMuscle": "Chest",
          "equipment": ["Barbell", "Bench"],
          "images": [
            {
              "id": "img1",
              "url": "/api/media/exercises/bench-press-1.jpg",
              "type": "IMAGE",
              "description": "Bench press starting position"
            }
          ],
          "videos": [],
          "orderIndex": 1
        }
      ]
    }
  ]
}

Response (404): No active plan exists
{
  "message": "No active workout plan found"
}
```

**2. Generate New Plan**

```
POST /api/workout-plans/generate
Authorization: Bearer {token}
Body: {} (empty - uses member profile from token)

Response (200): Same structure as GET active plan
- Archives current active plan (if exists)
- Generates new plan based on member's fitness profile
- Returns complete plan with all exercises and media
```

### Split Type Details (For UI Display)

| Split Type       | Days | Day Structure                                                                | Description                          |
| ---------------- | ---- | ---------------------------------------------------------------------------- | ------------------------------------ |
| `FULL_BODY`      | 3    | Day 1-3: Full Body                                                           | All major muscle groups each session |
| `UPPER_LOWER`    | 4    | Day 1: Upper, Day 2: Lower, Day 3: Upper, Day 4: Lower                       | Alternating upper/lower body         |
| `BRO_SPLIT_4DAY` | 4    | Day 1: Chest+Tri, Day 2: Back+Bi, Day 3: Shoulders, Day 4: Legs              | Classic 4-day bodybuilding split     |
| `BRO_SPLIT_5DAY` | 5    | Day 1: Chest, Day 2: Back, Day 3: Shoulders, Day 4: Legs, Day 5: Arms        | 5-day bodybuilding split             |
| `PUSH_PULL_LEGS` | 6    | Day 1: Push, Day 2: Pull, Day 3: Legs, Day 4: Push, Day 5: Pull, Day 6: Legs | Push/pull/legs rotation              |

### Muscle Group Labels (Common Values)

- **Push:** Chest, Shoulders, Triceps
- **Pull:** Back, Biceps, Rear Delts
- **Legs:** Quads, Hamstrings, Glutes, Calves
- **Chest & Triceps:** Chest-focused with tricep work
- **Back & Biceps:** Back-focused with bicep work
- **Upper Body:** Full upper body workout
- **Lower Body:** Full lower body workout

### Equipment Categories (For Filtering/Display)

- **Bodyweight:** No equipment needed
- **Dumbbell:** Dumbbells required
- **Barbell:** Barbell/bench required
- **Machine:** Gym machines
- **Cable:** Cable machines
- **Bands:** Resistance bands

---

## 10. WORKFLOW SCENARIOS

### Scenario 1: First Time Visit (No Plan)

```
User visits /member-workout-plan
  ↓
useWorkoutPlan() → getActivePlan() → 404
  ↓
Show: "No workout plan yet!"
  ↓
[Generate Your First Plan] button
  ↓
Click → generateNewPlan() → API
  ↓
Show new plan
  ↓
[Start Workout] available
```

### Scenario 2: View Existing Plan

```
User visits /member-workout-plan
  ↓
useWorkoutPlan() → getActivePlan() → Success
  ↓
Display: Plan summary + days/exercises
  ↓
Click on exercise → ExerciseMediaViewer opens
  ↓
View images, click [Next Exercise]
  ↓
[Start Workout] → Begin progress tracking
```

### Scenario 3: Track Workout Session

```
User clicks [Start Workout]
  ↓
useWorkoutProgress() → Load or create new session
  ↓
WorkoutProgressTracker shows current exercise
  ↓
User enters sets/reps data
  ↓
Click [Mark Complete]
  ↓
Save to localStorage → Move to next exercise
  ↓
Continue until day complete
  ↓
Option to [Finish Workout] or [Save & Continue Later]
```

### Scenario 4: Regenerate Plan

```
User clicks [Generate New Plan]
  ↓
RegenerateModal → Confirmation
  ↓
Click [Generate]
  ↓
generateNewPlan() API call
  ↓
Archive old plan, create new
  ↓
Clear localStorage progress
  ↓
Display new plan
```

---

## 11. STYLING & THEME

### Color Scheme

- Primary: Use app's primary color for CTAs
- Secondary: For badges/tags
- Success: Green for completed exercises ✓
- Warning: Orange for "in progress"
- Neutral: Gray for archived/completed days

### Component Styling

- Cards with subtle shadows
- Tab navigation (horizontal scroll on mobile)
- Responsive grid for exercise cards
- Modal overlays with animations
- Progress bars with smooth transitions

### Typography

- Headers: Bold, larger for plan name
- Subheaders: Medium weight for day labels
- Body: Regular for exercise details
- Accent: Bold/colored for important numbers (sets × reps)

---

## 12. RESPONSIVE DESIGN

### Mobile (< 768px)

- Stacked layout
- Full-width cards
- Tab scrolling for days
- Bottom action buttons
- Modal takes full screen

### Tablet (768px - 1024px)

- 2-column exercise grid
- Horizontal day tabs
- Side-by-side modals

### Desktop (> 1024px)

- 3-column exercise grid
- Day tabs + exercise list
- Split panel layout if needed

---

## 13. ACCESSIBILITY

- ✅ Keyboard navigation (tabs, modals)
- ✅ ARIA labels for images/videos
- ✅ Color contrast meets WCAG AA
- ✅ Progress indicators clearly announced
- ✅ Form labels for input fields
- ✅ Semantic HTML structure

---

## 14. PERFORMANCE OPTIMIZATION

- Lazy load images/videos
- Cache API responses with SWR/React Query
- Optimize re-renders with useMemo
- Pagination for large exercise lists (if needed)
- Service worker for offline progress tracking (optional)

---

## 15. ERROR HANDLING

| Error            | User Message                             | Action                         |
| ---------------- | ---------------------------------------- | ------------------------------ |
| No active plan   | "Create your first workout plan"         | Show generate button           |
| API error        | "Failed to load plan. Please try again." | Retry button                   |
| Network error    | "Check your connection"                  | Offline mode with localStorage |
| Unauthorized     | Auto-redirect to login                   | -                              |
| Validation error | "Invalid exercise data"                  | Fallback UI                    |

---

## 16. TESTING CHECKLIST

- [ ] Load plan successfully
- [ ] Display all days and exercises
- [ ] Click exercise → Media viewer opens
- [ ] Scroll through exercise images
- [ ] Mark exercise as complete
- [ ] Progress bar updates
- [ ] Regenerate modal appears & confirms
- [ ] Generate new plan works
- [ ] Progress clears after generation
- [ ] Mobile responsive
- [ ] Error handling works
- [ ] Accessibility keyboard nav

---

## 17. DEPLOYMENT CHECKLIST

- [ ] All types defined
- [ ] Service methods tested
- [ ] Components render without errors
- [ ] API endpoints verified
- [ ] localStorage keys consistent
- [ ] Responsive design verified
- [ ] Error messages user-friendly
- [ ] Loading states show
- [ ] No console errors/warnings

---

## 18. FUTURE ENHANCEMENTS

1. **Exercise Modifications**
   - Swap exercises
   - Adjust sets/reps
   - Mark exercises as "not available"

2. **Advanced Tracking**
   - Weight progression tracking
   - Personal records (PRs)
   - Workout history/stats
   - Export workouts as PDF

3. **Social Features**
   - Share workout plan
   - Compare with friends
   - Leaderboards

4. **AI Integration**
   - Form feedback (if videos available)
   - Workout recommendations
   - Adaptive difficulty

5. **Offline Support**
   - Service worker
   - Sync progress when online

---

## Summary for AI Implementation

**Core Dependencies:**

- React, TypeScript
- Axios (API)
- Lucide React (icons)
- Tailwind CSS (styling)
- React Router (navigation)

**Start with:**

1. Create types/workoutPlan.ts
2. Create services/workoutPlanService.ts
3. Create hooks/useWorkoutPlan.ts & useWorkoutProgress.ts
4. Create components in workout/ folder
5. Create pages/MemberWorkoutPlan.tsx
6. Update router
7. Add styling & responsiveness

**Key Implementation Details:**

- Use localStorage for progress (key: `workout_progress_${planId}`)
- Handle 404 for "no plan" scenario
- Modals for media viewer & regenerate confirmation
- Tab-based day navigation
- Real-time progress calculation
