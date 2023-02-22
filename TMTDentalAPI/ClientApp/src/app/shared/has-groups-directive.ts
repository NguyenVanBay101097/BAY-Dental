import { Directive, ElementRef, Input, OnInit, Renderer2, RendererStyleFlags2 } from '@angular/core';
import { PermissionService } from './permission.service';

@Directive({
    selector: '[hasGroups]'
})
export class HasGroupsDirective implements OnInit {
    @Input() groups: string;
    @Input() item: any;
    @Input() child: any;
    constructor(private elementRef: ElementRef, private renderer: Renderer2,
        private permissionService: PermissionService,) {

    }

    ngOnInit() {
        this.permissionService.permissionStoreChangeEmitter.subscribe(() => {
            this.applyPermission();
        })

        this.applyPermission();
    }

    applyPermission() {
        if (!this.groups || this.groups == "product.group_uom") {
            return false;
        }

        var permissions = this.groups.split(',');
        let hasDefined = this.permissionService.hasOneDefined(permissions);

        if (!hasDefined) {
            this.renderer.setStyle(
                this.elementRef.nativeElement,
                'display',
                'none', RendererStyleFlags2.Important + RendererStyleFlags2.DashCase
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
