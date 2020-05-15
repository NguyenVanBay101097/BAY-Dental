import { NgControl } from '@angular/forms';
import { Directive, Input, ElementRef, Renderer2, OnInit } from '@angular/core';
import * as _ from 'lodash';
import { PermissionService } from './permission.service';

@Directive({
    selector: '[hasGroups]'
})
export class HasGroupsDirective implements OnInit {
    @Input() groups: string;
    constructor(private elementRef: ElementRef, private renderer: Renderer2,
        private permissionService: PermissionService) {

    }

    ngOnInit() {
        this.permissionService.permissionStoreChangeEmitter.subscribe(() => {
            this.applyPermission();
        })

        this.applyPermission();
    }

    applyPermission() {
        if (!this.groups) {
            return false;
        }
        var permissions = this.groups.split(',');
        let hasDefined = this.permissionService.hasOneDefined(permissions);

        if (!hasDefined) {
            this.renderer.setStyle(
                this.elementRef.nativeElement,
                'display',
                'none'
            )
        }
        else {
            this.renderer.setStyle(
                this.elementRef.nativeElement,
                'display',
                ''
            )
        }
    }
}
