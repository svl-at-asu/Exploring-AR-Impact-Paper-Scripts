// JavaScript source code
import * as d3 from "https://cdn.jsdelivr.net/npm/d3@7/+esm";

// Set the dimensions of the canvas / graph
var margin = { top: 10, right: 20, bottom: 50, left: 50 },
    width = 800 - margin.left - margin.right,
	height = 470 - margin.top - margin.bottom;

// Define function for parsing datetimes.
var parseTime = d3.timeParse("%m/%d/%Y %H:%M:%S %p");

// Set the time scale for the x axis and linear scale for the y axis.
const xScale = d3.scaleTime().range([0, width]);
const yScale = d3.scaleLinear().range([height, 0]);

// append the svg obgect to the body of the page
// appends a 'group' element to 'svg'
// moves the 'group' element to the top left margin
var svg = d3.select(".root").append("svg")
    .attr("width", width + margin.left + margin.right)
    .attr("height", height + margin.top + margin.bottom)
    .append("g")
    .attr("transform",
        "translate(" + margin.left + "," + margin.top + ")");

const data = await d3.csv("data/test.csv");
console.log(data);

// Format the data (parsing the strings to their appropriate datatypes)
data.forEach(function (d) {
	d.time = parseTime(d.time);
	d.p1_angle = Number(d.p1_angle);
});

// Set the time scale range based on the data's time range.
xScale.domain(d3.extent(data, function (d) { return d.time; }));
yScale.domain([0, 180]);

// Define the curve.
const p1_line = d3.line()
	.x((d) => { return xScale(d.time); })
	.y((d) => { return yScale(d.p1_angle); })
	.curve(d3.curveCardinal);

// Add the curve.
//const p1_path = svg.append("path")
//	.data(data)
//	.attr("class", "line")
//	.attr("d", p1_line);

svg.append("path").datum(data)
	.attr("fill", "none")
	.attr("stroke", "steelblue")
	.attr("stroke-linejoin", "round")
	.attr("stroke-linecap", "round")
	.attr("stroke-width", 1.5)
	.attr("d", p1_line);

// Define the x Axis
const xAxis = d3.axisBottom(xScale)
	.ticks(11, ".0s");

// Add the X Axis
svg.append("g")
	.attr("transform", "translate(0," + height + ")")
	.attr("font-family", "Lato")
	.call(xAxis);

// Add the X Axis Label
svg.append("text")
	.attr("font-family", "sans-serif")
	.attr("font-size", 14)
	.attr("font-weight", 700)
	.attr("text-anchor", "middle")
	.attr("x", width / 2)
	.attr("y", height + margin.bottom - 5)
	.text("Time");

// Add the Y Axis
svg.append("g")
	.call(d3.axisLeft(yScale));

// Add the Y Axis Label
svg.append("text")
	.attr("font-family", "sans-serif")
	.attr("font-size", 14)
	.attr("font-weight", 700)
	.attr("text-anchor", "middle")
	.attr("x", 0 - (height / 2))
	.attr("y", 10 - margin.left)
	.attr("transform", "rotate(-90)")
	.text("Angle");

// Add the title
svg.append("text")
	.attr("font-family", "sans-serif")
	.attr("font-size", 16)
	.attr("font-weight", 700)
	.attr("text-decoration", "underline")
	.attr("text-anchor", "middle")
	.attr("x", width / 2)
	.attr("y", 12 - margin.top)
	.text("Participant Angle Data");
