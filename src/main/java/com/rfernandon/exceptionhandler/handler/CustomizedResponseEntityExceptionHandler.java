package com.rfernandon.exceptionhandler.handler;

import com.rfernandon.exceptionhandler.exceptions.CallErrorException;
import com.rfernandon.exceptionhandler.utils.ErrorMessageFactory;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.HttpHeaders;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.lang.Nullable;
import org.springframework.stereotype.Component;
import org.springframework.web.bind.MethodArgumentNotValidException;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.bind.annotation.RestControllerAdvice;
import org.springframework.web.context.request.WebRequest;
import org.springframework.web.servlet.mvc.method.annotation.ResponseEntityExceptionHandler;

import static org.springframework.http.HttpStatus.INTERNAL_SERVER_ERROR;
import static org.springframework.http.MediaType.APPLICATION_JSON;

@Slf4j
@Component
@RestControllerAdvice
public class CustomizedResponseEntityExceptionHandler extends ResponseEntityExceptionHandler {

    /**
     * Método utilizado para interceptar as exceptions relacionadas com as validações dos dados (bean validate).
     */
    @Override
    protected ResponseEntity<Object> handleMethodArgumentNotValid(
            MethodArgumentNotValidException ex, HttpHeaders headers, HttpStatus status, WebRequest request) {
        var httpStatus = HttpStatus.UNPROCESSABLE_ENTITY;
        return getResponseEntity(ex, httpStatus, ErrorMessageFactory.error(httpStatus, ex.getBindingResult()));
    }

    @Override
    public ResponseEntity<Object> handleExceptionInternal(Exception ex, @Nullable Object body, HttpHeaders headers, HttpStatus status, WebRequest request) {
        return getResponseEntity(ex, status, ErrorMessageFactory
                .error(String.valueOf(status.value()), ex.getMessage()));
    }

    @ExceptionHandler(Exception.class)
    public ResponseEntity<Object> handleException(Exception ex) {
        var httpStatus = INTERNAL_SERVER_ERROR;
        return getResponseEntity(ex, httpStatus, ErrorMessageFactory
                .error(String.valueOf(httpStatus.value()), ex.getMessage()));
    }

    @ExceptionHandler(CallErrorException.class)
    public ResponseEntity<Object> handleException(CallErrorException ex) {
        return getResponseEntity(ex, ex.getHttpStatus(), ex.getMessage());
    }

    private ResponseEntity<Object> getResponseEntity(Exception ex, HttpStatus httpStatus, String mensagem) {
        log.error(ex.getMessage(), ex);
        return ResponseEntity.status(httpStatus).contentType(APPLICATION_JSON).body(mensagem);
    }
}
