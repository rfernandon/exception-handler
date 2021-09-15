package com.rfernandon.exceptionhandler.exceptions;

import static org.springframework.http.HttpStatus.PRECONDITION_FAILED;

public class PreconditionException extends ErrorException {

    public PreconditionException(String message) {
        super(message, PRECONDITION_FAILED);
    }
}