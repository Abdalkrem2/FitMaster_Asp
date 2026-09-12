import { api } from './api';
import type { WorkoutPlan, WorkoutDay, WorkoutExercise, WorkoutProgress } from '../types/workoutPlan';

// Backend field names (workoutDays/workoutExercises) differ from the frontend's
// established shape (days/exercises) - mapped here so every existing component
// that already consumes WorkoutPlan doesn't need to change.
function mapPlan(raw: any): WorkoutPlan {
  return {
    id: raw.id,
    name: raw.name,
    splitType: raw.splitType,
    goal: raw.goal,
    level: raw.level,
    status: raw.status,
    createdAt: raw.createdAt,
    days: (raw.workoutDays ?? []).map(
      (d: any): WorkoutDay => ({
        id: d.id,
        dayNumber: d.dayNumber,
        muscleGroupLabel: d.muscleGroupLabel,
        exercises: (d.workoutExercises ?? []).map(
          (e: any): WorkoutExercise => ({
            id: e.id,
            exerciseName: e.exerciseName ?? 'Exercise',
            difficulty: e.difficulty,
            sets: e.sets,
            reps: e.reps,
            repsMax: e.repsMax,
            durationSeconds: e.durationSeconds,
            targetMuscles: e.targetMuscles,
            primaryMuscle: e.primaryMuscle,
            equipment: e.equipment,
            imageUrl: e.imageUrl,
            orderIndex: e.orderIndex,
          }),
        ),
      }),
    ),
  };
}

export const workoutPlanService = {
  async getActivePlan(): Promise<WorkoutPlan> {
    const { data } = await api.get('/workoutplans/me');
    return mapPlan(data);
  },

  async getPlanHistory(): Promise<WorkoutPlan[]> {
    const { data } = await api.get('/workoutplans/me/history');
    return (data ?? []).map(mapPlan);
  },

  async generateNewPlan(): Promise<WorkoutPlan> {
    await api.post('/workoutplans/me/generate', {});
    return workoutPlanService.getActivePlan();
  },

  async downloadPlanPdf(): Promise<Blob> {
    const response = await api.get('/workoutplans/me/active/pdf', {
      responseType: 'blob',
    });
    return new Blob([response.data], { type: 'application/pdf' });
  },

  saveProgress(progress: WorkoutProgress): void {
    localStorage.setItem(`workout_progress_${progress.planId}`, JSON.stringify(progress));
  },

  loadProgress(planId: number): WorkoutProgress | null {
    try {
      const raw = localStorage.getItem(`workout_progress_${planId}`);
      return raw ? (JSON.parse(raw) as WorkoutProgress) : null;
    } catch {
      return null;
    }
  },

  clearProgress(planId: number): void {
    localStorage.removeItem(`workout_progress_${planId}`);
  },

  getProgressPercent(planId: number, totalExercises: number): number {
    if (totalExercises === 0) return 0;
    const progress = workoutPlanService.loadProgress(planId);
    if (!progress) return 0;
    const done = progress.completedExercises.filter((e) => e.completed).length;
    return Math.round((done / totalExercises) * 100);
  },
};
