package events

import (
	"encoding/json"
	"log"
	"v1/models"

	"github.com/streadway/amqp"
)

type EventProducer struct {
	conn    *amqp.Connection
	channel *amqp.Channel
}

func NewEventProducer(amqpURI string) (*EventProducer, error) {
	conn, err := amqp.Dial(amqpURI)
	if err != nil {
		return nil, err
	}

	ch, err := conn.Channel()
	if err != nil {
		return nil, err
	}

	err = ch.ExchangeDeclare(
		"reservation_events", // name
		"topic",              // type
		true,                 // durable
		false,                // auto-deleted
		false,                // internal
		false,                // no-wait
		nil,                  // arguments
	)
	if err != nil {
		return nil, err
	}

	return &EventProducer{
		conn:    conn,
		channel: ch,
	}, nil
}

func (p *EventProducer) Close() error {
	if err := p.channel.Close(); err != nil {
		return err
	}
	return p.conn.Close()
}

func (p *EventProducer) PublishReservationCreated(reservation models.Reservation) error {
	body, err := json.Marshal(reservation)
	if err != nil {
		return err
	}

	err = p.channel.Publish(
		"reservation_events",  // exchange
		"reservation.created", // routing key
		false,                 // mandatory
		false,                 // immediate
		amqp.Publishing{
			ContentType: "application/json",
			Body:        body,
		})

	if err != nil {
		log.Printf("Failed to publish message: %v", err)
	} else {
		log.Printf("Message published successfully")
	}

	return err
}
