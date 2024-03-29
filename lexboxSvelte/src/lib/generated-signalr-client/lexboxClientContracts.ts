/* THIS (.ts) FILE IS GENERATED BY Tapper */
/* eslint-disable */
/* tslint:disable */

/** Transpiled from lexboxClientContracts.Entry */
export type Entry = {
    /** Transpiled from System.Guid */
    id: string;
    /** Transpiled from lexboxClientContracts.MultiString */
    lexemeForm: MultiString;
    /** Transpiled from lexboxClientContracts.MultiString */
    citationForm: MultiString;
    /** Transpiled from lexboxClientContracts.MultiString */
    literalMeaning: MultiString;
    /** Transpiled from System.Collections.Generic.IList<lexboxClientContracts.Sense> */
    senses: Sense[];
    /** Transpiled from lexboxClientContracts.MultiString */
    note: MultiString;
}

/** Transpiled from lexboxClientContracts.ExampleSentence */
export type ExampleSentence = {
    /** Transpiled from System.Guid */
    id: string;
    /** Transpiled from lexboxClientContracts.MultiString */
    sentence: MultiString;
    /** Transpiled from lexboxClientContracts.MultiString */
    translation: MultiString;
    /** Transpiled from string */
    reference: string;
}

/** Transpiled from lexboxClientContracts.QueryOptions */
export type QueryOptions = {
    /** Transpiled from string */
    order: string;
    /** Transpiled from int */
    count: number;
    /** Transpiled from int */
    offset: number;
}

/** Transpiled from lexboxClientContracts.MultiString */
export type MultiString = {
    /** Transpiled from System.Collections.Generic.IDictionary<lexboxClientContracts.WritingSystemId, string> */
    values: { [key: WritingSystemId]: string };
}

/** Transpiled from lexboxClientContracts.Sense */
export type Sense = {
    /** Transpiled from System.Guid */
    id: string;
    /** Transpiled from lexboxClientContracts.MultiString */
    definition: MultiString;
    /** Transpiled from lexboxClientContracts.MultiString */
    gloss: MultiString;
    /** Transpiled from string */
    partOfSpeech: string;
    /** Transpiled from System.Collections.Generic.IList<string> */
    semanticDomain: string[];
    /** Transpiled from System.Collections.Generic.IList<lexboxClientContracts.ExampleSentence> */
    exampleSentences: ExampleSentence[];
}

/** Transpiled from lexboxClientContracts.WritingSystem */
export type WritingSystem = {
    /** Transpiled from lexboxClientContracts.WritingSystemId */
    id: WritingSystemId;
    /** Transpiled from string */
    name: string;
    /** Transpiled from string */
    abbreviation: string;
    /** Transpiled from string */
    font: string;
}

/** Transpiled from lexboxClientContracts.WritingSystems */
export type WritingSystems = {
    /** Transpiled from lexboxClientContracts.WritingSystem[] */
    analysis: WritingSystem[];
    /** Transpiled from lexboxClientContracts.WritingSystem[] */
    vernacular: WritingSystem[];
}

/** Transpiled from lexboxClientContracts.WritingSystemId */
export type WritingSystemId = string;

