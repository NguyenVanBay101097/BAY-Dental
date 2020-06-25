import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ToaThuocLinesSaveCuFormComponent } from './toa-thuoc-lines-save-cu-form.component';

describe('ToaThuocLinesSaveCuFormComponent', () => {
  let component: ToaThuocLinesSaveCuFormComponent;
  let fixture: ComponentFixture<ToaThuocLinesSaveCuFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ToaThuocLinesSaveCuFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ToaThuocLinesSaveCuFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
