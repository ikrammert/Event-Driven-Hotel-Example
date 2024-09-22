package models

type Reservation struct {
	ID        string `json:"id"`
	HotelID   string `json:"hotel_id"`
	GuestName string `json:"guest_name"`
}
