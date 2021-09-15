package com.rfernandon.exceptionhandler.exceptions;

import static org.springframework.http.HttpStatus.NOT_IMPLEMENTED;

public class NotImplementedException extends ErrorException {

    public NotImplementedException(String message) {
        super(message, NOT_IMPLEMENTED);
    }
}