# Exploring-AR-Impact-Paper-Scripts

This repository contains the scripts used to process and analyze the data for the "Exploring AR Impact" paper [final title pending].

## Data Reader Input Formats

The data reader has a "Read" method associated with each of the four raw data input tables. Data should be stored in CSV files. The format each of these tables must be in is defined as follows:

### Gestures Data
`Team, Video, Time, Action, Gesturer, Target, Intent`

### Looks Data
`Team, Video, Time, Initiator`

### Utterances Data
`Team, Video, Time, Purpose, SPeaker, Deictic Pronouns, Spatial Deictic`

### Utterances Count Data
`Team, Video, Trial, P1 Speaking to Self, P2 Speaking to Self, P1 Speaking to P2, P2 Speaking to P1, Either Person Speaking to Instructor`

### Trial Recording Data
`Team, Trial, Modality, Chart Type, Task Type, Task Number, Video, Start, End`

### Transformed Data Output Format
`Team, Trial, Modality, Trial Time, Event Type, Participant, Action, Action Target, Action Intent, Utterance Purpose`
