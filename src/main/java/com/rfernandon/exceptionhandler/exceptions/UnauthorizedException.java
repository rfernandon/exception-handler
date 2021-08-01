package com.rfernandon.exceptionhandler.exceptions;

import org.springframework.http.HttpStatus;

public class UnauthorizedException extends CallErrorException {

    public UnauthorizedException(String message) {
        super(message, HttpStatus.UNAUTHORIZED);
    }
}
