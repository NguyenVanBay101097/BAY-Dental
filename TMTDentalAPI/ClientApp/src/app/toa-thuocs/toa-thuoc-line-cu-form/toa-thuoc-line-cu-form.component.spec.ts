import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ToaThuocLineCuFormComponent } from './toa-thuoc-line-cu-form.component';

describe('ToaThuocLineCuFormComponent', () => {
  let component: ToaThuocLineCuFormComponent;
  let fixture: ComponentFixture<ToaThuocLineCuFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ToaThuocLineCuFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ToaThuocLineCuFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
