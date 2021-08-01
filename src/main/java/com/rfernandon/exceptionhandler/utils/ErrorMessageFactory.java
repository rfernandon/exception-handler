package com.rfernandon.exceptionhandler.utils;

import com.rfernandon.exceptionhandler.model.ErrorDetails;
import org.springframework.http.HttpStatus;
import org.springframework.validation.BindingResult;

import java.text.MessageFormat;
import java.util.ArrayList;
import java.util.Collection;
import java.util.List;
import java.util.ResourceBundle;
import java.util.stream.Collectors;

import static com.rfernandon.exceptionhandler.utils.ParseUtil.objectToJson;

public class ErrorMessageFactory {

    private static String MESSAGES_PROPERTIES = "messages_en";

    private List<ErrorDetails> errorDetails = new ArrayList<>();

    public void add(String codeError, String message) {
        this.errorDetails.add(new ErrorDetails(codeError, message));
    }

    public void add(String codeError) {
        this.errorDetails.add(new ErrorDetails(codeError, getMessagesProperties(codeError)));
    }

    public String build() {
        return objectToJson(errorDetails);
    }

    public static String error(String codeError, String message) {
        var error = new ErrorDetails(codeError, message);
        return objectToJson(List.of(error));
    }

    public static String error(String codeError) {
        return error(codeError, getMessagesProperties(codeError));
    }

    public static String error(HttpStatus httpStatus, BindingResult bindingResult) {
        var error = getMessageValidation(bindingResult)
                .stream()
                .map(m -> new ErrorDetails(String.valueOf(httpStatus.value()), m))
                .collect(Collectors.toList());
        return objectToJson(error);
    }

    public static String errorWithParam(String codeError, String... params) {
        return error(codeError, params != null ? getMessagesProperties(codeError, params) : getMessagesProperties(codeError));
    }

    private static String getMessagesProperties(String codeError) {
        ResourceBundle resourceBundle = ResourceBundle.getBundle(MESSAGES_PROPERTIES);
        return resourceBundle.getString(codeError);
    }

    private static String getMessagesProperties(String codeError, String... params) {
        ResourceBundle resourceBundle = ResourceBundle.getBundle(MESSAGES_PROPERTIES);
        return MessageFormat.format(resourceBundle.getString(codeError), params);
    }

    private static Collection<String> getMessageValidation(BindingResult bindingResult) {
        Collection<String> l = new ArrayList<>();
        bindingResult.getFieldErrors().stream().forEach(f -> l.add(f.getField()+": "+f.getDefaultMessage()));
        return l;
    }
}
