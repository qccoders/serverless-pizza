package main

import (
	"context"
	"encoding/json"
	"errors"
	"fmt"
	"log"

	model "./model"

	"github.com/aws/aws-lambda-go/events"
	"github.com/aws/aws-lambda-go/lambda"
	"github.com/aws/aws-sdk-go/aws"
	"github.com/aws/aws-sdk-go/aws/session"
	"github.com/aws/aws-sdk-go/service/dynamodb"
	"github.com/aws/aws-sdk-go/service/dynamodb/dynamodbattribute"
)

var db = dynamodb.New(session.New())

func main() {
	lambda.Start(handler)
}

func handler(ctx context.Context, sqsEvent events.SQSEvent) error {
	if len(sqsEvent.Records) == 0 {
		return errors.New("No SQS message passed to function")
	}

	for _, msg := range sqsEvent.Records {
		msgBytes := []byte(msg.Body)
		order := model.Order{}

		err := json.Unmarshal(msgBytes, &order)

		if err != nil {
			log.Fatal(err)
		}

		fmt.Printf("event %q\n", order)

		originalRecord := get(order.ID, order.Name)

		json, _ := json.Marshal(originalRecord)

		fmt.Printf("dynamo record: %q", json)
	}

	return nil
}

func get(id string, name string) (order model.Order) {
	input := &dynamodb.GetItemInput{
		Key: map[string]*dynamodb.AttributeValue{
			"id": {
				S: aws.String(id),
			},
			"name": {
				S: aws.String(name),
			},
		},
		ConsistentRead: aws.Bool(true),
		TableName:      aws.String("serverless-pizza-orders"),
	}

	fmt.Printf("request: %s", input)

	result, err := db.GetItem(input)

	if err != nil {
		log.Fatal(err)
	}

	err = dynamodbattribute.UnmarshalMap(result.Item, &order)
	return order
}
