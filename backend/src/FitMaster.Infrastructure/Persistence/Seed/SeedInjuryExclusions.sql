-- Reference data: which seeded muscles to avoid loading for a given reported
-- injury (see Domain/Entities/Workouts/InjuryMuscleExclusion.cs). Run this
-- manually against the local DB the same way SeedExercises.sql is run, after
-- the injury_muscle_exclusions table exists (apply the EF migration first).
--
-- Matched by muscle name against the muscles table seeded in
-- SeedExercises.sql, not by hardcoded id, so it stays correct even if that
-- seed is re-run with different generated ids.

IF NOT EXISTS (SELECT 1 FROM injury_muscle_exclusions)
BEGIN
    INSERT INTO injury_muscle_exclusions (injury_type, muscle_id)
    SELECT v.injury_type, m.id
    FROM (VALUES
        (N'Knee', N'quadriceps'), (N'Knee', N'quads'), (N'Knee', N'hamstrings'),
        (N'Knee', N'glutes'), (N'Knee', N'adductors'), (N'Knee', N'abductors'),

        (N'Shoulder', N'shoulders'), (N'Shoulder', N'deltoids'), (N'Shoulder', N'delts'),
        (N'Shoulder', N'rear deltoids'), (N'Shoulder', N'rotator cuff'), (N'Shoulder', N'chest'),
        (N'Shoulder', N'pectorals'), (N'Shoulder', N'upper chest'), (N'Shoulder', N'triceps'),

        (N'LowerBack', N'lower back'), (N'LowerBack', N'spine'), (N'LowerBack', N'back'),

        (N'UpperBack', N'upper back'), (N'UpperBack', N'back'), (N'UpperBack', N'rhomboids'),
        (N'UpperBack', N'trapezius'), (N'UpperBack', N'traps'), (N'UpperBack', N'lats'),
        (N'UpperBack', N'latissimus dorsi'), (N'UpperBack', N'levator scapulae'),

        (N'Wrist', N'wrists'), (N'Wrist', N'wrist extensors'), (N'Wrist', N'wrist flexors'),
        (N'Wrist', N'forearms'), (N'Wrist', N'grip muscles'), (N'Wrist', N'hands'),

        (N'Ankle', N'ankles'), (N'Ankle', N'ankle stabilizers'), (N'Ankle', N'shins'),
        (N'Ankle', N'feet'), (N'Ankle', N'soleus'), (N'Ankle', N'calves'),

        (N'Neck', N'sternocleidomastoid'), (N'Neck', N'levator scapulae'),
        (N'Neck', N'trapezius'), (N'Neck', N'traps'),

        (N'Elbow', N'triceps'), (N'Elbow', N'biceps'), (N'Elbow', N'brachialis'), (N'Elbow', N'forearms'),

        (N'Hip', N'hip flexors'), (N'Hip', N'glutes'), (N'Hip', N'adductors'),
        (N'Hip', N'abductors'), (N'Hip', N'groin')
    ) AS v(injury_type, muscle_name)
    JOIN muscles m ON m.name = v.muscle_name;
END
