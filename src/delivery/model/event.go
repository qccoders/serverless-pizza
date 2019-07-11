package model

type Event struct {
	EventType string `dynamodbav:"size"`
	Start     string `dynamodbav:"start"`
	End       int    `dynamodbav:"end"`
}
