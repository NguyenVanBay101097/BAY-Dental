import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PrintTemplateConfigCuComponent } from './print-template-config-cu.component';

describe('PrintTemplateConfigCuComponent', () => {
  let component: PrintTemplateConfigCuComponent;
  let fixture: ComponentFixture<PrintTemplateConfigCuComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PrintTemplateConfigCuComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PrintTemplateConfigCuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
