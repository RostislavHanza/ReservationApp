import { useState } from "react";
import { useForm } from "react-hook-form";
import ContentCopyIcon from "@mui/icons-material/ContentCopy";
import {
    createReservation,
    changeReservation,
    deleteReservation,
    getReservation,
} from "./services/reservationApi";

import {
    Alert,
    Box,
    Button,
    Container,
    IconButton,
    InputAdornment,
    Paper,
    Step,
    StepLabel,
    Stepper,
    TextField,
    Typography
} from "@mui/material";

const steps = ["Welcome", "Reservation", "Confirmation"];
type ReservationForm = {
    email: string;
    phone: string;
    ticketCount: number;
};

function App() {
    const [activeStep, setActiveStep] = useState(0);
    const [editMode, setEditMode] = useState(false);

    const [showEditCode, setShowEditCode] = useState(false);
    const [reservationCodeInput, setReservationCodeInput] = useState("");
    const [resultType, setResultType] = useState<"created" | "changed" | "deleted">("created");

    const [reservationData, setReservationData] =
        useState<ReservationForm | null>(null);

    const [reservationCode, setReservationCode] =
        useState("");

    const [errorMessage, setErrorMessage] = useState("");

    const {
        register,
        handleSubmit,
        reset,
        formState: { errors }
    } = useForm<ReservationForm>();

    async function loadReservation() {
        try {
            setErrorMessage("");

            const reservation = await getReservation(reservationCodeInput);

            setEditMode(true);
            setReservationCode(reservationCodeInput);

            reset({
                email: reservation.email,
                phone: reservation.phone,
                ticketCount: reservation.ticketCount
            });

            setActiveStep(1);
        }
        catch (error) {
            if (error instanceof Error) {
                setErrorMessage(error.message);
            }
            else {
                setErrorMessage("Something went wrong.");
            }
        }
    }

    async function handleDelete() {
        const confirmed = window.confirm(
            "Are you sure you want to delete this reservation?"
        );

        if (!confirmed) {
            return;
        }

        try {
            setErrorMessage("");

            await deleteReservation(reservationCode);

            reset();
            setReservationCode("");
            setEditMode(false);
            setResultType("deleted");
            nextStep();
        }
        catch (error) {
            if (error instanceof Error) {
                setErrorMessage(error.message);
            }
            else {
                setErrorMessage("Something went wrong.");
            }
        }
    }

    async function submitReservation(data: ReservationForm) {

        setErrorMessage("");

        if (editMode) {
            try {
                await changeReservation({
                    email: data.email,
                    phone: data.phone,
                    ticketCount: data.ticketCount,
                    reservationCode: reservationCode
                });

                setReservationData(data);
                setResultType("changed");
                nextStep();
            }
            catch (error) {
                if (error instanceof Error) {
                    setErrorMessage(error.message);
                }
                else {
                    setErrorMessage("Something went wrong.");
                }
            }
            return;
        }
        try {
            const code = await createReservation({
                email: data.email,
                phone: data.phone,
                ticketCount: data.ticketCount,
            });

            setReservationData(data);
            setReservationCode(code);
            setResultType("created");
            nextStep();
        }
        catch (error) {
            if (error instanceof Error) {
                setErrorMessage(error.message);
            }
            else {
                setErrorMessage("Something went wrong.");
            }
        }
    }
    function nextStep() {
        setActiveStep((current) => current + 1);
    }

    function resetWizard() {
        setActiveStep(0);
        setEditMode(false);
        setShowEditCode(false);
        setReservationCodeInput("");
        setReservationCode("");
        setReservationData(null);
        setErrorMessage("");
        reset();
    }

    return (
        <Container maxWidth="sm">
            <Paper sx={{ mt: 6, p: 4 }}>
                <Typography variant="h4" gutterBottom>
                    Event Reservation
                </Typography>

                <Typography sx={{ mb: 4 }}>
                    December 31, 2027 at 19:00
                </Typography>

                <Stepper activeStep={activeStep} sx={{ mb: 4 }}>
                    {steps.map((step) => (
                        <Step key={step}>
                            <StepLabel>{step}</StepLabel>
                        </Step>
                    ))}
                </Stepper>

                {activeStep === 0 && (
                    <Box>
                        <Typography
                            variant="h5"
                            sx={{ mb: 3 }}
                        >
                            Welcome
                        </Typography>

                        <Button
                            variant="contained"
                            onClick={() => {
                                setEditMode(false);
                                setErrorMessage("");
                                reset();
                                setActiveStep(1);
                            }}
                        >
                            New Reservation
                        </Button>

                        <Button
                            variant="outlined"
                            sx={{ ml: 2 }}
                            onClick={() => setShowEditCode(true)}
                        >
                            Edit Reservation
                        </Button>

                        {showEditCode && (
                            <Box sx={{ mt: 3 }}>
                                <TextField
                                    fullWidth
                                    label="Reservation code"
                                    value={reservationCodeInput}
                                    onChange={(e) =>
                                        setReservationCodeInput(e.target.value)
                                    }
                                />

                                <Button
                                    variant="contained"
                                    sx={{ mt: 2 }}
                                    onClick={loadReservation}
                                >
                                    Continue
                                </Button>
                            </Box>
                        )}

                        {errorMessage && (
                            <Alert severity="error" sx={{ mt: 2 }}>
                                {errorMessage}
                            </Alert>
                        )}
                    </Box>
                )}

                {activeStep === 1 && (
                    <Box
                        component="form"
                        onSubmit={handleSubmit(submitReservation)}
                    >
                        <Typography variant="h6" gutterBottom>
                            Reservation
                        </Typography>

                        <TextField
                            fullWidth
                            label="Email"
                            sx={{ mb: 2 }}
                            error={!!errors.email}
                            helperText={errors.email?.message}
                            {...register("email", {
                                required: "Email is required.",
                                pattern: {
                                    value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                                    message: "Enter a valid email address."
                                }
                            })}
                        />

                        <TextField
                            fullWidth
                            label="Phone"
                            sx={{ mb: 2 }}
                            error={!!errors.phone}
                            helperText={errors.phone?.message}
                            {...register("phone", {
                                required: "Phone is required.",
                                pattern: {
                                    value: /^[0-9]{9}$/,
                                    message: "Enter exactly 9 digits without country prefix."
                                }
                            })}
                        />

                        <TextField
                            fullWidth
                            type="number"
                            label="Number of tickets"
                            sx={{ mb: 3 }}
                            error={!!errors.ticketCount}
                            helperText={errors.ticketCount?.message}
                            {...register("ticketCount", {
                                required: "Number of tickets is required.",
                                valueAsNumber: true,
                                min: {
                                    value: 1,
                                    message: "Minimum is 1 ticket."
                                },
                                max: {
                                    value: 10,
                                    message: "Maximum is 10 tickets."
                                }
                            })}
                        />
                        {errorMessage && (
                            <Alert severity="error" sx={{ mb: 2 }}>
                                {errorMessage}
                            </Alert>
                        )}
                        <Box sx={{ display: "flex", gap: 2 }}>
                            <Button
                                type="button"
                                onClick={resetWizard}
                            >
                                Back
                            </Button>

                            {editMode && (
                                <Button
                                    type="button"
                                    color="error"
                                    variant="outlined"
                                    onClick={handleDelete}
                                >
                                    Delete reservation
                                </Button>
                            )}

                            <Button
                                type="submit"
                                variant="contained"
                            >
                                {editMode ? "Save changes" : "Create reservation"}
                            </Button>
                        </Box>
                    </Box>
                )}

                {activeStep === 2 && (
                    <Box>
                        <Typography variant="h6" gutterBottom>
                            {resultType === "created" && "Reservation created"}
                            {resultType === "changed" && "Reservation changed"}
                            {resultType === "deleted" && "Reservation deleted"}
                        </Typography>

                        {resultType !== "deleted" && reservationData && (
                            <>
                                <Typography>
                                    Email: {reservationData.email}
                                </Typography>

                                <Typography>
                                    Phone: {reservationData.phone}
                                </Typography>

                                <Typography>
                                    Tickets: {reservationData.ticketCount}
                                </Typography>

                                <TextField
                                    fullWidth
                                    label="Reservation code"
                                    value={reservationCode}
                                    slotProps={{
                                        input: {
                                            readOnly: true,
                                            endAdornment: (
                                                <InputAdornment position="end">
                                                    <IconButton
                                                        edge="end"
                                                        onClick={() =>
                                                            navigator.clipboard.writeText(reservationCode)
                                                        }
                                                    >
                                                        <ContentCopyIcon />
                                                    </IconButton>
                                                </InputAdornment>
                                            )
                                        }
                                    }}
                                    sx={{
                                        mt: 3,
                                        "& .MuiInputBase-input": {
                                            textAlign: "center",
                                            fontSize: "1.2rem",
                                            fontWeight: 600,
                                            letterSpacing: "0.08em"
                                        }
                                    }}
                                />
                            </>
                        )}

                        <Button sx={{ mt: 2 }} onClick={resetWizard}
                            variant="contained"
                        >
                            Finish

                        </Button>
                    </Box>
                )}
            </Paper>
        </Container>
    );
}

export default App;