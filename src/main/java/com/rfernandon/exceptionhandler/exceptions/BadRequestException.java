package com.rfernandon.exceptionhandler.exceptions;

import org.springframework.http.HttpStatus;

public class BadRequestException extends CallErrorException {

    public BadRequestException(String message) {
        super(message, HttpStatus.BAD_REQUEST);
    }
}
