/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { IEntry } from "./i-entry";
import { IMultiString } from "./i-multi-string";
import { ISense } from "./i-sense";

export interface Entry extends IEntry {
    id: string;
    lexemeForm: IMultiString;
    citationForm: IMultiString;
    literalMeaning: IMultiString;
    senses: ISense[];
    note: IMultiString;
}
