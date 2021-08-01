package com.rfernandon.exceptionhandler.exceptions;

import lombok.Getter;
import org.springframework.http.HttpStatus;

@Getter
public class CallErrorException extends RuntimeException {

    private HttpStatus httpStatus;

    public CallErrorException() {
        this.httpStatus = HttpStatus.INTERNAL_SERVER_ERROR;
    }

    public CallErrorException(String message) {
        super(message);
        this.httpStatus = HttpStatus.INTERNAL_SERVER_ERROR;
    }

    public CallErrorException(String message, HttpStatus httpStatus) {
        super(message);
        this.httpStatus = httpStatus;
    }
}
