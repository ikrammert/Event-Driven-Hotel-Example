package handlers

import (
	"net/http"
	"v1/events"
	"v1/models"

	"github.com/gin-gonic/gin"
)

type ReservationHandler struct {
	eventProducer *events.EventProducer
}

func NewReservationHandler(eventProducer *events.EventProducer) *ReservationHandler {
	return &ReservationHandler{eventProducer: eventProducer}
}

func (h *ReservationHandler) CreateReservation(c *gin.Context) {
	var reservation models.Reservation
	if err := c.ShouldBindJSON(&reservation); err != nil {
		c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
		return
	}

	// burada örneğin db'ye kayıt işlemleri yapılabilir

	// reservation created event
	if err := h.eventProducer.PublishReservationCreated(reservation); err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": "Failed to publish event"})
		return
	}

	c.JSON(http.StatusCreated, reservation)
}
