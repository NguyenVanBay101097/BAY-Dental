import { NgControl } from '@angular/forms';
import { Directive, Input, ElementRef, Renderer2, OnInit } from '@angular/core';
import * as _ from 'lodash';

@Directive({
    selector: '[hasGroups]'
})
export class HasGroupsDirective implements OnInit {
    @Input() groups: string;
    constructor(private elementRef: ElementRef, private renderer: Renderer2) {

    }

    ngOnInit() {
        var groups = [];
        if (localStorage.getItem('groups')) {
            groups = JSON.parse(localStorage.getItem('groups'));
        }

        if (this.groups) {
            var arr = this.groups.split(',');
            var intersect = _.intersection(groups, arr);
            var visible = intersect.length == arr.length;
            if (!visible) {
                this.renderer.setStyle(
                    this.elementRef.nativeElement,
                    'display',
                    'none'
                )
            } else {

            }
        } else {
        }
    }
}
