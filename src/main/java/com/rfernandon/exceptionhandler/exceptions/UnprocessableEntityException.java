package com.rfernandon.exceptionhandler.exceptions;

import org.springframework.http.HttpStatus;

public class UnprocessableEntityException extends CallErrorException {

    public UnprocessableEntityException(String message) {
        super(message, HttpStatus.UNPROCESSABLE_ENTITY);
    }
}
