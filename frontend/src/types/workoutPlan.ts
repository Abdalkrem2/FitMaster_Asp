export type SplitType = "FULL_BODY" | "UPPER_LOWER" | "PUSH_PULL_LEGS";
export type FitnessGoal =
  | "MUSCLE_GAIN"
  | "WEIGHT_LOSS"
  | "ENDURANCE"
  | "GENERAL_FITNESS";
export type FitnessLevel = "BEGINNER" | "INTERMEDIATE" | "ADVANCED";
export type Difficulty = "BEGINNER" | "INTERMEDIATE" | "ADVANCED";
export type MediaType = "IMAGE" | "VIDEO" | "GIF";

export interface MediaFile {
  id: string;
  url: string;
  type: MediaType;
  description?: string;
}

export interface WorkoutExercise {
  id: number;
  exerciseName: string;
  exerciseCode?: string;
  difficulty?: Difficulty;
  sets: number;
  reps: number;
  repsMax: number;
  durationSeconds?: number;
  targetMuscles?: string[];
  primaryMuscle?: string;
  equipment?: string[];
  images?: MediaFile[];
  videos?: MediaFile[];
  imageUrl?: string;
  orderIndex: number;
}

export interface WorkoutDay {
  id: number;
  dayNumber: number;
  muscleGroupLabel: string;
  exercises: WorkoutExercise[];
}

export interface WorkoutPlan {
  id: number;
  name: string;
  splitType: SplitType;
  goal: FitnessGoal;
  level: FitnessLevel;
  status: "ACTIVE" | "ARCHIVED";
  createdAt: string;
  days: WorkoutDay[];
}

export interface SetProgress {
  setNumber: number;
  repsCompleted: number;
  weight?: number;
  notes?: string;
}

export interface ExerciseProgress {
  exerciseId: number;
  completedSets: SetProgress[];
  notes?: string;
  completed: boolean;
  completedAt?: string;
}

export interface WorkoutProgress {
  planId: number;
  currentDayNumber: number;
  currentExerciseIndex: number;
  completedExercises: ExerciseProgress[];
  workoutStartTime: string;
  workoutEndTime?: string;
  status: "IN_PROGRESS" | "COMPLETED" | "PAUSED";
}
