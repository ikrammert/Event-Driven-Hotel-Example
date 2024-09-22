package main

import (
	"log"
	"net/http"
	"v1/events"
	"v1/handlers"

	"github.com/gin-gonic/gin"
)

func main() {
	r := gin.Default()

	eventProducer, err := events.NewEventProducer("amqp://guest:guest@localhost:5672/")
	if err != nil {
		log.Fatalf("Failed to create event producer: %v", err)
	}
	defer eventProducer.Close()

	reservationHandler := handlers.NewReservationHandler(eventProducer)

	r.POST("/reservations", reservationHandler.CreateReservation)

	if err := http.ListenAndServe(":8053", r); err != nil {
		log.Fatalf("Failed to start server: %v", err)
	}
}
