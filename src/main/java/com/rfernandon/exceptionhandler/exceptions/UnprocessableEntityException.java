package com.rfernandon.exceptionhandler.exceptions;

import static org.springframework.http.HttpStatus.UNPROCESSABLE_ENTITY;

public class UnprocessableEntityException extends ErrorException {

    public UnprocessableEntityException(String message) {
        super(message, UNPROCESSABLE_ENTITY);
    }
}