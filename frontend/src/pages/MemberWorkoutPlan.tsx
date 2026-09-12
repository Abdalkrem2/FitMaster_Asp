import { useState, useEffect, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import {
  Dumbbell,
  RefreshCw,
  Play,
  ChevronLeft,
  ChevronRight,
  X,
  CheckCircle2,
  AlertCircle,
  Loader2,
  Zap,
  Target,
  BarChart3,
  Calendar,
  Image as ImageIcon,
  ChevronDown,
  Trophy,
  Flame,
  Clock,
  Bookmark,
  HelpCircle,
  Download,
} from "lucide-react";
import { workoutPlanService } from "../services/workoutPlanService";
import type {
  WorkoutPlan,
  WorkoutDay,
  WorkoutExercise,
  WorkoutProgress,
  ExerciseProgress,
} from "../types/workoutPlan";

// ─── Helpers ──────────────────────────────────────────────────────────────────

const SPLIT_LABELS: Record<string, string> = {
  FULL_BODY: "Full Body",
  UPPER_LOWER: "Upper / Lower",
  BRO_SPLIT_4DAY: "Bro Split (4 Day)",
  BRO_SPLIT_5DAY: "Bro Split (5 Day)",
  PUSH_PULL_LEGS: "Push Pull Legs",
};

const GOAL_LABELS: Record<string, string> = {
  MUSCLE_GAIN: "Muscle Gain",
  WEIGHT_LOSS: "Weight Loss",
  ENDURANCE: "Endurance",
  GENERAL_FITNESS: "General Fitness",
};

const LEVEL_LABELS: Record<string, string> = {
  BEGINNER: "Beginner",
  INTERMEDIATE: "Intermediate",
  ADVANCED: "Advanced",
};

const DIFF_STYLES: Record<string, string> = {
  EASY: "bg-emerald-50 text-emerald-600 border-emerald-100",
  MEDIUM: "bg-amber-50 text-amber-600 border-amber-100",
  HARD: "bg-rose-50 text-rose-600 border-rose-100",
};

function totalExercises(plan: WorkoutPlan) {
  return plan.days.reduce((sum, d) => sum + d.exercises.length, 0);
}

function timeAgo(iso: string) {
  const diff = Date.now() - new Date(iso).getTime();
  const days = Math.floor(diff / 86_400_000);
  if (days === 0) return "Today";
  if (days === 1) return "Yesterday";
  return `${days} days ago`;
}

// ─── Image Protection Helpers ─────────────────────────────────────────────────

function handleImageContextMenu(e: React.MouseEvent) {
  e.preventDefault();
  e.stopPropagation();
}

function handleImageCopy(e: React.ClipboardEvent) {
  e.preventDefault();
  e.stopPropagation();
}

function handleImageDragStart(e: React.DragEvent) {
  e.preventDefault();
  e.stopPropagation();
}

// ─── ExerciseCard ─────────────────────────────────────────────────────────────

interface ExerciseCardProps {
  exercise: WorkoutExercise;
  index: number;
  isCurrent: boolean;
  isCompleted: boolean;
  isTracking: boolean;
  onView: () => void;
  onComplete: () => void;
}

function ExerciseCard({
  exercise,
  index,
  isCurrent,
  isCompleted,
  isTracking,
  onView,
  onComplete,
}: ExerciseCardProps) {
  return (
    <div
      className={`relative bg-white rounded-xl border flex flex-col overflow-hidden transition-all duration-200 group ${
        isCompleted
          ? "border-emerald-200 opacity-75"
          : isCurrent
            ? "border-indigo-400 ring-2 ring-indigo-400/20 shadow-md"
            : "border-slate-100 hover:border-slate-200 hover:shadow-sm"
      }`}
    >
      {/* Top icons */}
      <div className="absolute top-2 left-2 right-2 flex justify-between z-10 pointer-events-none">
        <div className="w-6 h-6 flex items-center justify-center text-slate-400 pointer-events-auto cursor-pointer hover:text-slate-700 transition-colors">
          <Bookmark className="w-3.5 h-3.5" />
        </div>
        <div
          onClick={(e) => {
            e.stopPropagation();
            onView();
          }}
          className="w-6 h-6 flex items-center justify-center text-slate-400 pointer-events-auto cursor-pointer hover:text-slate-700 transition-colors"
        >
          <HelpCircle className="w-3.5 h-3.5" />
        </div>
      </div>

      {/* Image container - clicks to view media */}
      <div
        className="w-full aspect-square bg-white relative cursor-pointer flex items-center justify-center select-none"
        onClick={onView}
        onContextMenu={handleImageContextMenu}
        onCopy={handleImageCopy}
        style={
          {
            userSelect: "none",
            WebkitUserSelect: "none",
          } as React.CSSProperties
        }
      >
        {exercise.imageUrl || (exercise.images?.length || 0) > 0 ? (
          <img
            src={exercise.imageUrl || exercise.images?.[0]?.url}
            alt={exercise.exerciseName}
            draggable={false}
            onContextMenu={handleImageContextMenu}
            onCopy={handleImageCopy}
            onDragStart={handleImageDragStart}
            className="w-[85%] h-[85%] object-contain group-hover:scale-105 transition-transform duration-500 select-none pointer-events-none"
            style={
              {
                userSelect: "none",
                WebkitUserSelect: "none",
              } as React.CSSProperties
            }
          />
        ) : (
          <Dumbbell className="w-10 h-10 text-slate-200" />
        )}

        {/* Status badges overlay */}
        {isCompleted && (
          <div className="absolute inset-0 bg-emerald-500/10 flex items-center justify-center backdrop-blur-[1px]">
            <div className="bg-emerald-500 text-white p-3 rounded-full shadow-lg">
              <CheckCircle2 className="w-6 h-6" />
            </div>
          </div>
        )}
      </div>

      {/* Content */}
      <div className="p-3 pt-2 flex-1 flex flex-col border-t border-slate-50">
        <h3
          className={`font-semibold text-sm leading-tight text-slate-800 line-clamp-1 mb-0.5 ${isCompleted ? "text-slate-500" : ""}`}
        >
          {exercise.exerciseName}
        </h3>
        <p className="text-[11px] text-slate-400 truncate">
          {exercise.primaryMuscle || "Exercise"}
        </p>

        {/* Tracking Action (if active) */}
        {isTracking && !isCompleted && isCurrent && (
          <div className="mt-auto pt-3">
            <button
              type="button"
              onClick={(e) => {
                e.stopPropagation();
                onComplete();
              }}
              className="w-full flex items-center justify-center gap-1.5 py-1.5 rounded-lg bg-indigo-500 text-white text-[11px] font-bold hover:bg-indigo-600 transition-all shadow-sm shadow-indigo-500/25"
            >
              <CheckCircle2 className="w-3.5 h-3.5" />
              Mark Done
            </button>
          </div>
        )}
      </div>
    </div>
  );
}

// ─── MediaViewerModal ─────────────────────────────────────────────────────────

interface MediaViewerProps {
  exercise: WorkoutExercise;
  onClose: () => void;
  isCompleted: boolean;
  isTracking: boolean;
  onComplete: () => void;
  onNext?: () => void;
  onPrev?: () => void;
}

function MediaViewerModal({
  exercise,
  onClose,
  isCompleted,
  isTracking,
  onComplete,
  onNext,
  onPrev,
}: MediaViewerProps) {
  const [imgIdx, setImgIdx] = useState(0);
  const media =
    exercise.images && exercise.videos
      ? [...exercise.images, ...exercise.videos]
      : exercise.imageUrl
        ? [
            {
              id: String(exercise.id),
              url: exercise.imageUrl,
              type: "GIF" as const,
            },
          ]
        : [];

  return (
    <div
      className="fixed inset-0 z-50 flex items-end sm:items-center justify-center"
      onClick={onClose}
    >
      <div className="absolute inset-0 bg-black/60 backdrop-blur-sm" />
      <div
        className="relative bg-white w-full max-w-lg max-h-[92vh] sm:rounded-3xl overflow-y-auto shadow-2xl"
        onClick={(e) => e.stopPropagation()}
      >
        {/* Header */}
        <div className="sticky top-0 bg-white/95 backdrop-blur-md px-6 pt-5 pb-4 border-b border-slate-100 flex items-start justify-between gap-4 z-10">
          <div>
            <h2 className="text-xl font-black text-slate-800">
              {exercise.exerciseName}
            </h2>
            <p className="text-sm text-slate-400 mt-0.5">
              {exercise.primaryMuscle || "Exercise"}
            </p>
          </div>
          <button
            onClick={onClose}
            className="p-2 rounded-xl hover:bg-slate-100 text-slate-400 transition-colors shrink-0"
          >
            <X className="w-5 h-5" />
          </button>
        </div>

        <div className="p-6 space-y-5">
          {/* Image/Video Gallery */}
          {media.length > 0 ? (
            <div
              onContextMenu={handleImageContextMenu}
              onCopy={handleImageCopy}
              style={
                {
                  userSelect: "none",
                  WebkitUserSelect: "none",
                } as React.CSSProperties
              }
            >
              <div className="w-full aspect-video bg-slate-900 rounded-2xl overflow-hidden relative">
                {media[imgIdx]?.type === "VIDEO" ? (
                  <video
                    key={media[imgIdx]?.url}
                    src={media[imgIdx]?.url}
                    draggable={false}
                    onContextMenu={handleImageContextMenu}
                    onDragStart={handleImageDragStart}
                    className="w-full h-full object-contain select-none pointer-events-none"
                    style={
                      {
                        userSelect: "none",
                        WebkitUserSelect: "none",
                      } as React.CSSProperties
                    }
                    controls
                    autoPlay
                    loop
                    muted
                  />
                ) : (
                  <img
                    src={media[imgIdx]?.url}
                    alt={exercise.exerciseName}
                    draggable={false}
                    onContextMenu={handleImageContextMenu}
                    onCopy={handleImageCopy}
                    onDragStart={handleImageDragStart}
                    className="w-full h-full object-contain select-none pointer-events-none"
                    style={
                      {
                        userSelect: "none",
                        WebkitUserSelect: "none",
                      } as React.CSSProperties
                    }
                  />
                )}
                {/* Prev/Next on image */}
                {media.length > 1 && (
                  <>
                    <button
                      onClick={() => setImgIdx((i) => Math.max(0, i - 1))}
                      className="absolute left-2 top-1/2 -translate-y-1/2 w-9 h-9 rounded-full bg-black/40 text-white flex items-center justify-center hover:bg-black/60 transition-colors"
                    >
                      <ChevronLeft className="w-5 h-5" />
                    </button>
                    <button
                      onClick={() =>
                        setImgIdx((i) => Math.min(media.length - 1, i + 1))
                      }
                      className="absolute right-2 top-1/2 -translate-y-1/2 w-9 h-9 rounded-full bg-black/40 text-white flex items-center justify-center hover:bg-black/60 transition-colors"
                    >
                      <ChevronRight className="w-5 h-5" />
                    </button>
                  </>
                )}
              </div>
              {media.length > 1 && (
                <div className="flex gap-2 mt-3 overflow-x-auto pb-1">
                  {media.map((m, i) => (
                    <button
                      key={m.id}
                      onClick={() => setImgIdx(i)}
                      className={`w-14 h-14 rounded-xl overflow-hidden border-2 shrink-0 transition-all ${
                        i === imgIdx
                          ? "border-indigo-500"
                          : "border-transparent opacity-60"
                      }`}
                    >
                      <img
                        src={m.url}
                        alt=""
                        draggable={false}
                        onContextMenu={handleImageContextMenu}
                        onCopy={handleImageCopy}
                        onDragStart={handleImageDragStart}
                        className="w-full h-full object-cover select-none"
                        style={
                          {
                            userSelect: "none",
                            WebkitUserSelect: "none",
                          } as React.CSSProperties
                        }
                      />
                    </button>
                  ))}
                </div>
              )}
            </div>
          ) : (
            <div className="w-full aspect-video rounded-2xl bg-gradient-to-br from-indigo-50 to-violet-50 border border-indigo-100 flex flex-col items-center justify-center gap-3">
              <div className="w-16 h-16 rounded-2xl bg-indigo-100 flex items-center justify-center">
                <Dumbbell className="w-8 h-8 text-indigo-400" />
              </div>
              <p className="text-sm text-slate-400 font-medium">
                No media available
              </p>
            </div>
          )}

          {/* Stats row */}
          <div className="grid grid-cols-3 gap-3">
            <div className="bg-indigo-50 rounded-xl p-3 text-center">
              <p className="text-2xl font-black text-indigo-600">
                {exercise.sets}
              </p>
              <p className="text-xs text-slate-400 mt-0.5">Sets</p>
            </div>
            <div className="bg-indigo-50 rounded-xl p-3 text-center">
              <p className="text-2xl font-black text-indigo-600">
                {exercise.reps === exercise.repsMax
                  ? exercise.reps
                  : `${exercise.reps}–${exercise.repsMax}`}
              </p>
              <p className="text-xs text-slate-400 mt-0.5">Reps</p>
            </div>
            {exercise.difficulty && (
              <div
                className={`rounded-xl p-3 text-center ${DIFF_STYLES[exercise.difficulty]}`}
              >
                <p className="text-2xl font-black">{exercise.difficulty[0]}</p>
                <p className="text-xs mt-0.5 opacity-70">
                  {exercise.difficulty}
                </p>
              </div>
            )}
          </div>

          {/* Muscles */}
          {(exercise.targetMuscles?.length || 0) > 0 && (
            <div>
              <p className="text-xs font-semibold text-slate-400 uppercase tracking-wider mb-2">
                Target Muscles
              </p>
              <div className="flex flex-wrap gap-2">
                {(exercise.targetMuscles || []).map((m) => (
                  <span
                    key={m}
                    className="px-3 py-1 rounded-full bg-indigo-50 text-indigo-600 text-sm font-medium"
                  >
                    {m}
                  </span>
                ))}
              </div>
            </div>
          )}

          {/* Equipment */}
          {(exercise.equipment?.length || 0) > 0 && (
            <div>
              <p className="text-xs font-semibold text-slate-400 uppercase tracking-wider mb-2">
                Equipment
              </p>
              <div className="flex flex-wrap gap-2">
                {(exercise.equipment || []).map((eq) => (
                  <span
                    key={eq}
                    className="px-3 py-1 rounded-full bg-slate-100 text-slate-600 text-sm font-medium"
                  >
                    {eq}
                  </span>
                ))}
              </div>
            </div>
          )}

          {/* Actions */}
          <div className="flex items-center gap-3 pt-2">
            {onPrev && (
              <button
                onClick={onPrev}
                className="flex items-center gap-1.5 px-4 py-2.5 rounded-xl border border-slate-200 text-sm font-semibold text-slate-600 hover:bg-slate-50 transition-all"
              >
                <ChevronLeft className="w-4 h-4" />
                Prev
              </button>
            )}
            {isTracking && !isCompleted && (
              <button
                onClick={() => {
                  onComplete();
                  onClose();
                }}
                className="flex-1 flex items-center justify-center gap-2 px-4 py-2.5 rounded-xl bg-gradient-to-r from-indigo-500 to-violet-600 text-white text-sm font-bold hover:opacity-90 transition-all shadow-sm shadow-indigo-500/25"
              >
                <CheckCircle2 className="w-4 h-4" />
                Mark Done
              </button>
            )}
            {isCompleted && (
              <div className="flex-1 flex items-center justify-center gap-2 px-4 py-2.5 rounded-xl bg-emerald-50 text-emerald-600 text-sm font-bold">
                <CheckCircle2 className="w-4 h-4" />
                Completed
              </div>
            )}
            {onNext && (
              <button
                onClick={onNext}
                className="flex items-center gap-1.5 px-4 py-2.5 rounded-xl border border-slate-200 text-sm font-semibold text-slate-600 hover:bg-slate-50 transition-all"
              >
                Next
                <ChevronRight className="w-4 h-4" />
              </button>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}

// ─── RegenerateModal ──────────────────────────────────────────────────────────

interface RegenerateModalProps {
  isLoading: boolean;
  onConfirm: () => void;
  onCancel: () => void;
}

function RegenerateModal({
  isLoading,
  onConfirm,
  onCancel,
}: RegenerateModalProps) {
  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center px-4"
      onClick={onCancel}
    >
      <div className="absolute inset-0 bg-black/50 backdrop-blur-sm" />
      <div
        className="relative bg-white rounded-3xl shadow-2xl w-full max-w-sm p-8 text-center"
        onClick={(e) => e.stopPropagation()}
      >
        <div className="w-16 h-16 rounded-2xl bg-gradient-to-br from-indigo-100 to-violet-100 flex items-center justify-center mx-auto mb-5">
          <RefreshCw className="w-8 h-8 text-indigo-500" />
        </div>
        <h2 className="text-xl font-black text-slate-800">
          Generate New Plan?
        </h2>
        <p className="text-slate-400 text-sm mt-2 leading-relaxed">
          Your current plan will be archived. You'll get a fresh set of
          exercises built around the same goal and split.
        </p>
        <div className="flex gap-3 mt-7">
          <button
            onClick={onCancel}
            disabled={isLoading}
            className="flex-1 py-3 rounded-2xl border border-slate-200 text-sm font-semibold text-slate-600 hover:bg-slate-50 transition-all disabled:opacity-50"
          >
            Cancel
          </button>
          <button
            onClick={onConfirm}
            disabled={isLoading}
            className="flex-1 py-3 rounded-2xl bg-gradient-to-r from-indigo-500 to-violet-600 text-white text-sm font-bold hover:opacity-90 transition-all disabled:opacity-50 flex items-center justify-center gap-2 shadow-sm shadow-indigo-500/25"
          >
            {isLoading ? (
              <Loader2 className="w-4 h-4 animate-spin" />
            ) : (
              <RefreshCw className="w-4 h-4" />
            )}
            {isLoading ? "Generating…" : "Generate"}
          </button>
        </div>
      </div>
    </div>
  );
}

// ─── Loading Skeleton ─────────────────────────────────────────────────────────

function LoadingSkeleton() {
  return (
    <div className="space-y-6 animate-pulse">
      <div className="h-8 bg-slate-200 rounded-xl w-56" />
      <div className="bg-white rounded-2xl border border-slate-100 p-6 space-y-4">
        <div className="flex gap-3">
          <div className="h-6 bg-slate-200 rounded-full w-28" />
          <div className="h-6 bg-slate-200 rounded-full w-24" />
          <div className="h-6 bg-slate-200 rounded-full w-20" />
        </div>
        <div className="h-4 bg-slate-100 rounded w-40" />
        <div className="flex gap-3 pt-2">
          <div className="h-10 bg-slate-200 rounded-xl w-36" />
          <div className="h-10 bg-slate-200 rounded-xl w-32" />
        </div>
      </div>
      {[1, 2, 3].map((i) => (
        <div
          key={i}
          className="bg-white rounded-2xl border border-slate-100 p-5"
        >
          <div className="flex gap-4">
            <div className="w-9 h-9 bg-slate-200 rounded-xl shrink-0" />
            <div className="flex-1 space-y-2">
              <div className="h-4 bg-slate-200 rounded w-48" />
              <div className="h-3 bg-slate-100 rounded w-32" />
              <div className="h-3 bg-slate-100 rounded w-24 mt-2" />
            </div>
          </div>
        </div>
      ))}
    </div>
  );
}

// ─── Main Component ───────────────────────────────────────────────────────────

export default function MemberWorkoutPlan() {
  const navigate = useNavigate();

  // Plan state
  const [plan, setPlan] = useState<WorkoutPlan | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [hasNoPlan, setHasNoPlan] = useState(false);

  // UI state
  const [activeDay, setActiveDay] = useState(0);
  const [showRegen, setShowRegen] = useState(false);
  const [regenLoading, setRegenLoading] = useState(false);
  const [generating, setGenerating] = useState(false);
  const [downloading, setDownloading] = useState(false);
  const [mediaExercise, setMediaExercise] = useState<{
    exercise: WorkoutExercise;
    dayIdx: number;
    exIdx: number;
  } | null>(null);

  // Progress tracking
  const [progress, setProgress] = useState<WorkoutProgress | null>(null);
  const [isTracking, setIsTracking] = useState(false);

  // ── Fetch plan ──────────────────────────────────────────────────────────────
  const fetchPlan = useCallback(async () => {
    setLoading(true);
    setError(null);
    setHasNoPlan(false);
    try {
      const data = await workoutPlanService.getActivePlan();
      setPlan(data);
      const saved = workoutPlanService.loadProgress(data.id);
      if (saved && saved.status === "IN_PROGRESS") {
        setProgress(saved);
        setIsTracking(true);
        const dayIdx = data.days.findIndex(
          (d) => d.dayNumber === saved.currentDayNumber,
        );
        if (dayIdx >= 0) setActiveDay(dayIdx);
      }
    } catch (err) {
      if (axios.isAxiosError(err)) {
        if (err.response?.status === 404) {
          setHasNoPlan(true);
        } else if (
          err.response?.status === 401 ||
          err.response?.status === 403
        ) {
          navigate("/login");
        } else {
          setError("Failed to load your workout plan. Please try again.");
        }
      } else {
        setError("Unable to connect. Check your internet connection.");
      }
    } finally {
      setLoading(false);
    }
  }, [navigate]);

  useEffect(() => {
    fetchPlan();
  }, [fetchPlan]);

  // ── Generate plan ───────────────────────────────────────────────────────────
  const doGenerate = async () => {
    setRegenLoading(true);
    try {
      const newPlan = await workoutPlanService.generateNewPlan();
      if (plan) workoutPlanService.clearProgress(plan.id);
      setPlan(newPlan);
      setHasNoPlan(false);
      setProgress(null);
      setIsTracking(false);
      setActiveDay(0);
      setShowRegen(false);
    } catch {
      setError("Failed to generate a new plan. Please try again.");
      setShowRegen(false);
    } finally {
      setRegenLoading(false);
      setGenerating(false);
    }
  };

  const handleGenerateFirst = async () => {
    setGenerating(true);
    await doGenerate();
  };

  const handleDownloadPlan = async () => {
    if (!plan) return;
    setDownloading(true);
    setError(null);
    try {
      const pdfBlob = await workoutPlanService.downloadPlanPdf();
      const url = URL.createObjectURL(pdfBlob);
      const link = document.createElement("a");
      link.href = url;
      link.download = `${plan.name}.pdf`;
      document.body.appendChild(link);
      link.click();
      link.remove();
      URL.revokeObjectURL(url);
    } catch (err) {
      setError("Unable to download the plan. Please try again.");
    } finally {
      setDownloading(false);
    }
  };

  // ── Workout tracking ────────────────────────────────────────────────────────
  const startWorkout = () => {
    if (!plan) return;
    const day = plan.days[activeDay];
    const newProgress: WorkoutProgress = {
      planId: plan.id,
      currentDayNumber: day.dayNumber,
      currentExerciseIndex: 0,
      completedExercises: [],
      workoutStartTime: new Date().toISOString(),
      status: "IN_PROGRESS",
    };
    setProgress(newProgress);
    setIsTracking(true);
    workoutPlanService.saveProgress(newProgress);
  };

  const completeExercise = (exerciseId: number) => {
    if (!plan || !progress) return;
    const ep: ExerciseProgress = {
      exerciseId,
      completedSets: [],
      completed: true,
      completedAt: new Date().toISOString(),
    };
    const existing = progress.completedExercises.find(
      (e) => e.exerciseId === exerciseId,
    );
    const updated: WorkoutProgress = {
      ...progress,
      completedExercises: existing
        ? progress.completedExercises.map((e) =>
            e.exerciseId === exerciseId ? ep : e,
          )
        : [...progress.completedExercises, ep],
    };
    setProgress(updated);
    workoutPlanService.saveProgress(updated);
  };

  const stopWorkout = () => {
    if (plan && progress) {
      workoutPlanService.saveProgress({ ...progress, status: "PAUSED" });
    }
    setIsTracking(false);
    setProgress(null);
  };

  const finishWorkout = () => {
    if (plan && progress) {
      workoutPlanService.saveProgress({
        ...progress,
        status: "COMPLETED",
        workoutEndTime: new Date().toISOString(),
      });
    }
    setIsTracking(false);
    setProgress(null);
  };

  // ── Computed values ─────────────────────────────────────────────────────────
  const completedIds = new Set(
    (progress?.completedExercises ?? [])
      .filter((e) => e.completed)
      .map((e) => e.exerciseId),
  );
  const currentDay = plan?.days[activeDay];
  const dayCompleted = currentDay
    ? currentDay.exercises.every((ex) => completedIds.has(ex.id))
    : false;
  const total = plan ? totalExercises(plan) : 0;
  const doneCount = completedIds.size;
  const progressPercent = total > 0 ? Math.round((doneCount / total) * 100) : 0;

  // Media viewer helpers
  const openMedia = (dayIdx: number, exIdx: number) => {
    if (!plan) return;
    setMediaExercise({
      exercise: plan.days[dayIdx].exercises[exIdx],
      dayIdx,
      exIdx,
    });
  };
  const mediaNavPrev = () => {
    if (!plan || !mediaExercise) return;
    const { dayIdx, exIdx } = mediaExercise;
    if (exIdx > 0) {
      setMediaExercise({
        exercise: plan.days[dayIdx].exercises[exIdx - 1],
        dayIdx,
        exIdx: exIdx - 1,
      });
    }
  };
  const mediaNavNext = () => {
    if (!plan || !mediaExercise) return;
    const { dayIdx, exIdx } = mediaExercise;
    const day = plan.days[dayIdx];
    if (exIdx < day.exercises.length - 1) {
      setMediaExercise({
        exercise: day.exercises[exIdx + 1],
        dayIdx,
        exIdx: exIdx + 1,
      });
    }
  };

  // ── Render ──────────────────────────────────────────────────────────────────

  return (
    <div className="space-y-6 animate-in fade-in slide-in-from-bottom-4 duration-500">
      {/* Page Header */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 bg-white p-6 rounded-2xl shadow-sm border border-slate-100">
        <div>
          <h1 className="text-2xl font-bold text-slate-800 flex items-center gap-2">
            <Dumbbell className="w-6 h-6 text-indigo-600" />
            My Workout Plan
          </h1>
          <p className="text-sm text-slate-500 mt-1">
            Your personalised training programme
          </p>
        </div>
        {plan && (
          <button
            onClick={() => setShowRegen(true)}
            className="flex-1 sm:flex-none flex items-center justify-center gap-2 px-4 py-2 bg-indigo-600 text-white rounded-xl hover:bg-indigo-700 transition-all font-medium text-sm shadow-sm shadow-indigo-600/20"
          >
            <RefreshCw className="w-4 h-4" />
            Generate New Plan
          </button>
        )}
      </div>

      {/* ── Loading ── */}
      {loading && <LoadingSkeleton />}

      {/* ── Error ── */}
      {!loading && error && (
        <div className="bg-white rounded-2xl border border-rose-100 p-8 text-center space-y-4">
          <div className="w-14 h-14 rounded-2xl bg-rose-50 flex items-center justify-center mx-auto">
            <AlertCircle className="w-7 h-7 text-rose-400" />
          </div>
          <div>
            <p className="font-bold text-slate-800">Something went wrong</p>
            <p className="text-sm text-slate-400 mt-1">{error}</p>
          </div>
          <button
            onClick={fetchPlan}
            className="inline-flex items-center gap-2 px-5 py-2.5 rounded-xl bg-gradient-to-r from-indigo-500 to-violet-600 text-white text-sm font-bold hover:opacity-90 transition-all shadow-sm shadow-indigo-500/25"
          >
            <RefreshCw className="w-4 h-4" />
            Try Again
          </button>
        </div>
      )}

      {/* ── No Plan ── */}
      {!loading && !error && hasNoPlan && (
        <div className="bg-white rounded-3xl border border-slate-100 shadow-sm overflow-hidden">
          <div className="h-1.5 bg-gradient-to-r from-indigo-500 to-violet-600" />
          <div className="p-10 text-center space-y-5">
            <div className="w-20 h-20 rounded-3xl bg-gradient-to-br from-indigo-100 to-violet-100 flex items-center justify-center mx-auto">
              <Dumbbell className="w-10 h-10 text-indigo-400" />
            </div>
            <div>
              <h2 className="text-2xl font-black text-slate-800">
                No active workout plan found
              </h2>
              <p className="text-slate-400 mt-2 max-w-sm mx-auto text-sm leading-relaxed">
                No active workout plan found. Generate one now to start your
                personalised training programme.
              </p>
            </div>
            <button
              onClick={handleGenerateFirst}
              disabled={generating}
              className="inline-flex items-center gap-2.5 px-8 py-3.5 rounded-2xl bg-gradient-to-r from-indigo-500 to-violet-600 text-white font-bold hover:opacity-90 transition-all shadow-md shadow-indigo-500/25 disabled:opacity-70"
            >
              {generating ? (
                <>
                  <Loader2 className="w-5 h-5 animate-spin" /> Generating…
                </>
              ) : (
                <>
                  <Flame className="w-5 h-5" /> Generate My Plan
                </>
              )}
            </button>
          </div>
        </div>
      )}

      {/* ── Plan View ── */}
      {!loading && !error && plan && (
        <>
          {/* Plan summary card */}
          <div className="bg-white rounded-2xl border border-slate-100 shadow-sm overflow-hidden">
            <div className="h-1 bg-gradient-to-r from-indigo-500 to-violet-600" />
            <div className="p-5 sm:p-6">
              <div className="flex flex-col sm:flex-row sm:items-start sm:justify-between gap-4">
                <div className="space-y-3">
                  <h2 className="text-lg font-black text-slate-800">
                    {plan.name}
                  </h2>
                  <div className="flex flex-wrap gap-2">
                    <span className="flex items-center gap-1.5 text-xs font-semibold px-3 py-1 rounded-full bg-indigo-50 text-indigo-600">
                      <BarChart3 className="w-3.5 h-3.5" />
                      {SPLIT_LABELS[plan.splitType] ?? plan.splitType}
                    </span>
                    <span className="flex items-center gap-1.5 text-xs font-semibold px-3 py-1 rounded-full bg-violet-50 text-violet-600">
                      <Target className="w-3.5 h-3.5" />
                      {GOAL_LABELS[plan.goal] ?? plan.goal}
                    </span>
                    <span className="flex items-center gap-1.5 text-xs font-semibold px-3 py-1 rounded-full bg-slate-100 text-slate-600">
                      <Zap className="w-3.5 h-3.5" />
                      {LEVEL_LABELS[plan.level] ?? plan.level}
                    </span>
                    <span className="flex items-center gap-1.5 text-xs font-medium px-3 py-1 rounded-full bg-slate-50 text-slate-400">
                      <Calendar className="w-3.5 h-3.5" />
                      {timeAgo(plan.createdAt)}
                    </span>
                  </div>

                  {/* Progress bar if tracking */}
                  {isTracking && (
                    <div className="space-y-1.5">
                      <div className="flex items-center justify-between text-xs">
                        <span className="font-semibold text-slate-600">
                          {doneCount} of {total} exercises done
                        </span>
                        <span className="font-bold text-indigo-600">
                          {progressPercent}%
                        </span>
                      </div>
                      <div className="h-2 bg-slate-100 rounded-full overflow-hidden">
                        <div
                          className="h-full bg-gradient-to-r from-indigo-500 to-violet-600 rounded-full transition-all duration-500"
                          style={{ width: `${progressPercent}%` }}
                        />
                      </div>
                    </div>
                  )}
                </div>

                {/* Action buttons */}
                <div className="flex flex-col gap-2 sm:items-end shrink-0">
                  {!isTracking ? (
                    <button
                      onClick={startWorkout}
                      className="flex items-center gap-2 px-5 py-2.5 rounded-xl bg-gradient-to-r from-indigo-500 to-violet-600 text-white text-sm font-bold hover:opacity-90 transition-all shadow-sm shadow-indigo-500/25"
                    >
                      <Play className="w-4 h-4" />
                      Start Workout
                    </button>
                  ) : (
                    <>
                      {dayCompleted ? (
                        <button
                          onClick={finishWorkout}
                          className="flex items-center gap-2 px-5 py-2.5 rounded-xl bg-emerald-500 text-white text-sm font-bold hover:opacity-90 transition-all shadow-sm shadow-emerald-500/25"
                        >
                          <Trophy className="w-4 h-4" />
                          Finish Workout
                        </button>
                      ) : (
                        <div className="flex items-center gap-2 px-4 py-2 rounded-xl bg-indigo-50 text-indigo-600 text-sm font-semibold">
                          <Loader2 className="w-4 h-4 animate-spin" />
                          Workout in progress
                        </div>
                      )}
                      <button
                        onClick={stopWorkout}
                        className="text-xs text-slate-400 hover:text-slate-600 transition-colors"
                      >
                        Save &amp; Continue Later
                      </button>
                    </>
                  )}
                  <button
                    onClick={handleDownloadPlan}
                    disabled={downloading}
                    className="flex items-center gap-2 px-4 py-2.5 rounded-xl border border-slate-200 text-sm font-semibold text-slate-600 hover:bg-slate-50 transition-all disabled:opacity-70"
                  >
                    <Download className="w-4 h-4" />
                    {downloading ? "Downloading…" : "Download Plan"}
                  </button>
                </div>
              </div>
            </div>
          </div>

          {/* Day Tabs */}
          <div className="overflow-x-auto -mx-4 sm:mx-0 px-4 sm:px-0">
            <div className="flex gap-2 pb-1 min-w-max sm:min-w-0 sm:flex-wrap">
              {plan.days.map((day, i) => {
                const dayDone = day.exercises.every((ex) =>
                  completedIds.has(ex.id),
                );
                return (
                  <button
                    key={day.dayNumber}
                    onClick={() => setActiveDay(i)}
                    className={`flex items-center gap-2 px-4 py-2.5 rounded-xl text-sm font-semibold whitespace-nowrap transition-all duration-200 ${
                      activeDay === i
                        ? "bg-gradient-to-r from-indigo-500 to-violet-600 text-white shadow-sm shadow-indigo-500/20"
                        : dayDone
                          ? "bg-emerald-50 text-emerald-600 border border-emerald-100"
                          : "bg-white border border-slate-200 text-slate-600 hover:bg-slate-50"
                    }`}
                  >
                    {dayDone && activeDay !== i && (
                      <CheckCircle2 className="w-3.5 h-3.5" />
                    )}
                    <span>Day {day.dayNumber}</span>
                    <span
                      className={`text-xs font-medium hidden sm:inline ${activeDay === i ? "text-indigo-100" : "text-slate-400"}`}
                    >
                      {day.muscleGroupLabel}
                    </span>
                  </button>
                );
              })}
            </div>
          </div>

          {/* Day Content */}
          {currentDay && (
            <div className="space-y-3">
              {/* Day header */}
              <div className="flex items-center justify-between">
                <div>
                  <h3 className="font-bold text-slate-800">
                    Day {currentDay.dayNumber}: {currentDay.muscleGroupLabel}
                  </h3>
                  <p className="text-sm text-slate-400">
                    {currentDay.exercises.length} exercises
                  </p>
                </div>
                {isTracking && (
                  <div className="text-xs font-semibold text-slate-400">
                    {
                      currentDay.exercises.filter((ex) =>
                        completedIds.has(ex.id),
                      ).length
                    }{" "}
                    /&nbsp;
                    {currentDay.exercises.length} done
                  </div>
                )}
              </div>

              {/* Exercise cards */}
              <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-5 gap-4">
                {currentDay.exercises
                  .sort((a, b) => a.orderIndex - b.orderIndex)
                  .map((exercise, exIdx) => {
                    const isDone = completedIds.has(exercise.id);
                    const isCurrent =
                      isTracking &&
                      !isDone &&
                      currentDay.exercises
                        .filter((ex) => !completedIds.has(ex.id))
                        .findIndex((ex) => ex.id === exercise.id) === 0;
                    return (
                      <ExerciseCard
                        key={exercise.id}
                        exercise={exercise}
                        index={exIdx}
                        isCurrent={isCurrent}
                        isCompleted={isDone}
                        isTracking={isTracking}
                        onView={() => openMedia(activeDay, exIdx)}
                        onComplete={() => completeExercise(exercise.id)}
                      />
                    );
                  })}
              </div>
            </div>
          )}
        </>
      )}

      {/* ── Media Viewer ── */}
      {mediaExercise && plan && (
        <MediaViewerModal
          exercise={mediaExercise.exercise}
          onClose={() => setMediaExercise(null)}
          isCompleted={completedIds.has(mediaExercise.exercise.id)}
          isTracking={isTracking}
          onComplete={() => completeExercise(mediaExercise.exercise.id)}
          onPrev={mediaExercise.exIdx > 0 ? mediaNavPrev : undefined}
          onNext={
            mediaExercise.exIdx <
            plan.days[mediaExercise.dayIdx].exercises.length - 1
              ? mediaNavNext
              : undefined
          }
        />
      )}

      {/* ── Regenerate Modal ── */}
      {showRegen && (
        <RegenerateModal
          isLoading={regenLoading}
          onConfirm={doGenerate}
          onCancel={() => setShowRegen(false)}
        />
      )}
    </div>
  );
}
