Supplemental Materials
Measuring and Comparing Collaborative Visualization Behaviors in Desktop and Augmented Reality Environments
VRST 2023 Submission

These supplemental materials are provided in the interest of verification and increasing reproducability of the study. A description of the included files follows.

Data processing scripts used with this data can be publicly found on GitHub at https://github.com/svl-at-asu/Exploring-AR-Impact-Paper-Scripts.



===================================================================================

ARLocationData

This folder contains the location data files from the Hololens for the 84 AR trials used in the calculations presented in the paper (the data from the remaining 36 trials was lost). Files are named with the team and trial number:

"Angles_Team[#]_Trial[#].csv"

This csv file has the following columns:

time		| p1_x		| p1_y		| p2_x		| p2_y		| p1_angle	| p2_angle	| distance

time		The time stamp of the data point
p1_x		The x position of participant 1 in meters
p1_y		The y position of participant 1 in meters
p2_x		The x position of participant 2 in meters
p2_y		The y position of participant 2 in meters
p1_angle	The angle formed between participant 2 and the visualization, with participant 1 as the vertex in degrees
p2_angle	The angle formed between participant 1 and the visualization, with participant 2 as the vertex in degrees
distance	The distance between the two participants in meters

*the visualization is a 1 meter cube, centered at (0.5, 2.5).



===================================================================================

DriverDesktopModality.csv

This file contains data on which participants controlled the mouse (referred to as the "driver") in the desktop trials.

This csv file has the following columns:

Team		| Modality	| Task Type	| Video		| Task		| Trial		| Driver

Team		The team the trial is for
Modality	The modality in the trial
Task Type	The task type in the trial
Video		The video file the trial is recorded in
Task		The task participants performed in the trial
Trial		The trial number
Driver		The participant who controlled the mouse. { 0 = Both, 1 = Participant 1, 2 = Participant 2 }



===================================================================================

EventPositioningOutput.csv

This file contains the data about when participant events (gestures, looks, and utterances) occured relative to which positional arrangement the participants were in. This data is computed by combining the data from the files in the "ARLocationData" folder, and the "VideoEncodingEventTable.csv" file. 

This csv file has the following columns:

Team		| Trial		| Modality	| Chart Type	| Task Type	| Task Number	| Start Time	| Time Separate	| Gestures Rate Separate	| Looks Rate Separate	| Utterances	| Rate Separate	| Time Mixed	| Gestures Rate Mixed	| Looks Rate Mixed	| Utterances Rate Mixed	| Time Same	| Gestures Rate Same	| Looks Rate Same	| Utterances Rate Same

Team				The team the trial is for
Trial				The trial number
Modality			The modality in the trial
Chart Type			The chart type in the trial
Task Type			The task type in the trial
Task Number			The task participants performed in the trial
Start Time			The time stamp the trial started at
Time Separate			The total time the team spent in the "Separate Space" arrangement during the trial, in seconds
Gestures Rate Separate		The rate the team gestured while in the "Separate Space" arrangement in gestures per second
Looks Rate Separate		The rate the team looked while in the "Separate Space" arrangement in looks per second
Utterances Rate Separate	The rate the team uttered while in the "Separate Space" arrangement in utterances per second
Time Mixed			The total time the team spent in the "Mixed Space" arrangement during the trial, in seconds
Gestures Rate Mixed		The rate the team gestured while in the "Mixed Space" arrangement in gestures per second
Looks Rate Mixed		The rate the team looked while in the "Mixed Space" arrangement in looks per second
Utterances Rate Mixed		The rate the team uttered while in the "Mixed Space" arrangement in utterances per second
Time Same			The total time the team spent in the "Same Space" arrangement during the trial, in seconds
Gestures Rate Same		The rate the team gestured while in the "Same Space" arrangement in gestures per second
Looks Rate Same			The rate the team looked while in the "Same Space" arrangement in looks per second
Utterances Rate Same		The rate the team uttered while in the "Same Space" arrangement in utterances per second



===================================================================================

PostStudySurveyResponses.csv

This file contains the participant responses to the post-study survey, including the NASA TLX and free-response questions. Participant data has been anonymized.

This csv file has the following columns (NOTE - columns have two rows of header - the second row contains the text of the survey questions):

Participant ID	| TLX Mental Demand Computer	| TLX Mental Demand HoloLens	| TLX Physical Demand Computer		| TLX Physical Demand HoloLens	| TLX Temporal Demand Computer	| TLX Temporal Demand HoloLens	| TLX Performance Computer	| TLX Performance HoloLens	| TLX Effort Computer	| TLX Effort HoloLens	| TLX Frustration Computer	| TLX Frustration HoloLens	| Device Preference	| Communication Ease

Participant ID			The anonymized ID assigned to the participant.
TLX Mental Demand Computer	The repsonse on a scale of 1 to 7 for mental demand in the desktop modality.
TLX Mental Demand HoloLens	The repsonse on a scale of 1 to 7 for mental demand in the HoloLens modality.
TLX Physical Demand Computer	The repsonse on a scale of 1 to 7 for physical demand in the desktop modality.
TLX Physical Demand HoloLens	The repsonse on a scale of 1 to 7 for physical demand in the HoloLens modality.
TLX Temporal Demand Computer	The repsonse on a scale of 1 to 7 for temporal demand in the desktop modality.
TLX Temporal Demand HoloLens	The repsonse on a scale of 1 to 7 for temporal demand in the HoloLens modality.
TLX Performance Computer	The repsonse on a scale of 1 to 7 for performance in the desktop modality.
TLX Performance HoloLens	The repsonse on a scale of 1 to 7 for performance in the HoloLens modality.
TLX Effort Computer		The repsonse on a scale of 1 to 7 for effort in the desktop modality.
TLX Effort HoloLens		The repsonse on a scale of 1 to 7 for effort in the HoloLens modality.
TLX Frustration Computer	The repsonse on a scale of 1 to 7 for frustration in the desktop modality.
TLX Frustration HoloLens	The repsonse on a scale of 1 to 7 for frustration in the HoloLens modality.
Device Preference		The free-response to device (modality) preference.
Communication Ease		The free-response to ease of communication between modalities.



===================================================================================

Qualitative Coding Instructions.pdf

This file contains the instructions given to the video coders to qualitatively code the study videos. The "VideoEncodingEventTable.csv" file was generated as the final output of this coding process.



===================================================================================

Qualitative Coding Procedure.pdf

This file contains the procedure used to qualitatively code the study videos. The "VideoEncodingEventTable.csv" file was generated as the final output of this coding process.




===================================================================================

TrialTimeData.csv

This file contains the data necessary to correlate the time stamps across devices (two HoloLens with the video recording device), and is used to extract the data in the files in the "ARLocationData" folder from the raw data logs on the HoloLens. The HoloLens were both comtinuously recording across all trials (including between trials), so this data is necessary to "clip" the HoloLens data down to just the data recorded during the trials.

This csv file has the following columns:

Team		| Trial		| Modality	| Chart Type	| Task Type	| Task Number	| Video		| Start		| End		| Trial Time	| Time (seconds)

Team			The team the trial is for
Trial			The trial number
Modality		The modality in the trial
Chart Type		The chart type in the trial
Task Type		The task type in the trial
Task Number		The task participants performed in the trial
Video			The video file the trial is recorded in
Start			The start time in the video file of the trial recording
End			The end time in the video file of the trial recording
Trial Time		The duration of the trial (in minutes and seconds)
Time (seconds)		The duration of the trial (in seconds)



===================================================================================

UtteranceCounts.csv

This file contains the data on utterance counts broken down based on who was speaking to whom. This data was generated during the video encoding process.

This csv file has the following columns:

Team		| Modality	| Task Type	| Video		| Trial		| P1 speaking to self		| P2 speaking to self	| P1 speaking to P2	| P2 speaking to P1	| Either person speaking to instructor

Team					The team the trial is for
Modality				The modality in the trial
Task Type				The task type in the trial
Video					The video file the trial is recorded in
Trial					The trial number
P1 speaking to self			Count of utterances where participant 1 is speaking to themself
P2 speaking to self			Count of utterances where participant 2 is speaking to themself
P1 speaking to P2			Count of utterances where participant 1 is speaking to participant 2
P2 speaking to P1			Count of utterances where participant 2 is speaking to participant 1
Either person speaking to instructor	Count of utterances where either participant is speaking to the instructor



===================================================================================

VideoEncodingEventTable.csv

This file contains the data on all qualitatively coded participant events: Utterances, Gestures, and Looks. This data was generated during the video encoding process.

This csv file has the following columns:

Team	 	| Trial	 	| Modality	 | Chart Type	 | Task Type	 | Task Number	 | Trial Time	 | Event Type	 | Participant	 | Action	 | Action Target	 | Action Intent	 | Utterance Purpose	 | Deictic Pronouns	 | Spatial Deictic


Team			The team the trial is for
Trial			The trial number
Modality		The modality in the trial
Chart Type		The chart type in the trial
Task Type		The task type in the trial
Task Number		The task participants performed in the trial
Trial Time		The number of seconds into the trial the event occured
Event Type		The event type
Participant		The participant who performed the event
Action			The gesture the participant performed (only used for Gesture event type)
Action Target		The intended target of the gesture (only used for Gesture event type)
Action Intent		The intent the gesture was to communicate (only used for Gesture event type)
Utterance Purpose	The purpose of the utterance (only used for Utterance event type)
Deictic Pronouns	A count of the diectic pronouns used in the utterance (only used for Utterance event type)
Spatial Deictic		A count of the spatial diectic pronouns used in the utterance (only used for Utterance event type)

