export type FitnessGoal =
  | "MUSCLE_GAIN"
  | "WEIGHT_LOSS"
  | "ENDURANCE"
  | "GENERAL_FITNESS";

export type FitnessLevel = "BEGINNER" | "INTERMEDIATE" | "ADVANCED";

export type InjuryType =
  | "KNEE"
  | "SHOULDER"
  | "LOWER_BACK"
  | "UPPER_BACK"
  | "WRIST"
  | "ANKLE"
  | "NECK"
  | "ELBOW"
  | "HIP";

// Only three splits are supported by the workout generator (see backend Phase 6 notes) -
// the old BRO_SPLIT_4DAY/5DAY values from the previous backend no longer exist.
export type SplitType = "FULL_BODY" | "UPPER_LOWER" | "PUSH_PULL_LEGS";

export type TrainingStyle = "STRENGTH" | "HYPERTROPHY" | "CIRCUIT";

export type AllergyType =
  | "GLUTEN"
  | "LACTOSE"
  | "NUTS"
  | "EGGS"
  | "SHELLFISH"
  | "SOY";

export interface MemberProfile {
  goal: FitnessGoal;
  fitnessLevel: FitnessLevel;
  injuries?: InjuryType[];
  weight?: number;
  height?: number;
  age?: number;
  trainingStyle?: TrainingStyle;
  splitType?: SplitType;
  hasDiabetes?: boolean;
  hasHeartConditions?: boolean;
  hasHypertension?: boolean;
  allergies?: AllergyType[];
}

// Matches MemberListItemDto (GET /api/members)
export interface Member {
  memberId: number;
  fullName: string;
  phone: string;
  gender?: string;
  addedByName?: string;
  debt?: number;
  endDate?: string;
}

// Matches MemberDto (GET /api/members/{id} and GET /api/members/me)
export interface MemberDetails {
  memberId: number;
  phone: string;
  fullName: string;
  profilePicture?: string;
  gender?: string;
  createdAt: string;
  debt?: number;
  endDate?: string;
  goal: FitnessGoal;
  fitnessLevel: FitnessLevel;
  splitType: SplitType;
  trainingStyle?: TrainingStyle;
  injuries: InjuryType[];
  weight?: number;
  height?: number;
  age?: number;
  hasDiabetes: boolean;
  hasHeartConditions: boolean;
  hasHypertension: boolean;
  allergies: AllergyType[];
}

export interface CreateMemberRequest {
  fullName: string;
  phone: string;
  password: string;
  gender?: string;
  goal: FitnessGoal;
  fitnessLevel: FitnessLevel;
  splitType: SplitType;
  trainingStyle?: TrainingStyle;
  injuries?: InjuryType[];
  weight?: number;
  height?: number;
  age?: number;
  hasDiabetes?: boolean;
  hasHeartConditions?: boolean;
  hasHypertension?: boolean;
  allergies?: AllergyType[];
}

// Matches UpdateMemberIdentityCommand (PATCH /api/members/{id})
export interface UpdateMemberRequest {
  memberId: number;
  fullName: string;
  phone: string;
}

// Matches UpdateMemberProfileCommand (PUT /api/members/{id}, PATCH /api/members/me/profile)
export interface UpdateMemberProfileRequest {
  memberId?: number;
  goal: FitnessGoal;
  fitnessLevel: FitnessLevel;
  splitType: SplitType;
  trainingStyle?: TrainingStyle;
  injuries?: InjuryType[] | null;
  weight?: number | null;
  height?: number | null;
  age?: number | null;
  hasDiabetes?: boolean;
  hasHeartConditions?: boolean;
  hasHypertension?: boolean;
  allergies?: AllergyType[];
}
