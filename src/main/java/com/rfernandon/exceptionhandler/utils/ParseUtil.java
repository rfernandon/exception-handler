package com.rfernandon.exceptionhandler.utils;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.DeserializationFeature;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.SerializationFeature;
import com.fasterxml.jackson.databind.type.CollectionType;
import com.fasterxml.jackson.databind.type.TypeFactory;
import com.fasterxml.jackson.datatype.jsr310.JavaTimeModule;

import java.util.List;

import static java.util.Objects.nonNull;

public class ParseUtil {

    public static <T> T convertValue(Object value, Class<T> templateClass) {
        if (nonNull(value)) {
            var mapper = new ObjectMapper();
            mapper.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, false);
            mapper.configure(SerializationFeature.FAIL_ON_EMPTY_BEANS, false);
            mapper.configure(SerializationFeature.WRITE_DATES_AS_TIMESTAMPS, false);
            mapper.registerModule(new JavaTimeModule());
            return mapper.convertValue(value, templateClass);
        }
        return null;
    }

    public static <T> List<T> convertValues(Object value, Class<T> templateClass) {
        if (nonNull(value)) {
            var mapper = new ObjectMapper();
            mapper.configure(DeserializationFeature.ACCEPT_SINGLE_VALUE_AS_ARRAY, true);


            mapper.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, false);
            mapper.configure(SerializationFeature.FAIL_ON_EMPTY_BEANS, false);
            mapper.configure(SerializationFeature.WRITE_DATES_AS_TIMESTAMPS, false);
            mapper.registerModule(new JavaTimeModule());

            CollectionType collectionType = TypeFactory.defaultInstance().constructCollectionType(List.class, templateClass);
            List<T> converted = (List)mapper.convertValue(value, collectionType);
            return converted;
        }
        return null;
    }

    public static String objectToJson(Object object) {

        try {

            var mapper = new ObjectMapper();
            mapper.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, false);
            mapper.configure(SerializationFeature.FAIL_ON_EMPTY_BEANS, false);
            mapper.configure(SerializationFeature.WRITE_DATES_AS_TIMESTAMPS, false);
            mapper.registerModule(new JavaTimeModule());

            return mapper.writeValueAsString(object);

        } catch (JsonProcessingException e) {
            throw new IllegalArgumentException(e);
        }
    }
}
