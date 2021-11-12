import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WarrantyCuDidalogComponent } from './warranty-cu-didalog.component';

describe('WarrantyCuDidalogComponent', () => {
  let component: WarrantyCuDidalogComponent;
  let fixture: ComponentFixture<WarrantyCuDidalogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WarrantyCuDidalogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WarrantyCuDidalogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
